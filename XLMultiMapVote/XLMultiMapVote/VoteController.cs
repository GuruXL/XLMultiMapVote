using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModIO.UI;
using System;
using SkaterXL.Data;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using XLMultiMapVote.Map;
using XLMultiMapVote.State;
using XLMultiMapVote.Network;
using HarmonyLib;
using Photon.Pun;

namespace XLMultiMapVote
{
    public class VoteController : MonoBehaviour
    {
        private Action<int> popUpCallBack;

        //public string[] popUpOptions = new string[] { "Option 0", "Option 1", "Option 2", "Option 3", };
        public string[] popUpOptions = new string[] { };

        private Dictionary<int, int> voteIndex = new Dictionary<int, int>();

        public VoteState voteState { get; private set; }

        private Coroutine changeMapCoroutine;
        private Coroutine updateVoteListCoroutine;

        public void AddVoteState(GameObject obj)
        {
            if (voteState == null)
            {
                voteState = obj.AddComponent<VoteState>();
            }
            else
            {
                Main.Logger.Log("[AddVoteState] VoteState Already Exists");
            }
        }
        public void StartMapChangeRoutines()
        {
            if (changeMapCoroutine == null)
            {
                changeMapCoroutine = StartCoroutine(ChangeMap());

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "Change Map Started", 1.5f);
            }

            if (updateVoteListCoroutine == null)
            {
                updateVoteListCoroutine = StartCoroutine(UpdateVoteList());

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "Update Vote List Started", 1.5f);
            }
        }
        public void StopMapChangeRoutines()
        {
            if (changeMapCoroutine != null)
            {
                StopCoroutine(changeMapCoroutine);
                changeMapCoroutine = null; // Set to null so it can be restarted again

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "Change Map Stopped", 1.5f);
            }

            if (updateVoteListCoroutine != null)
            {
                StopCoroutine(updateVoteListCoroutine);
                updateVoteListCoroutine = null; // Set to null so it can be restarted again

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "Update Vote List Stopped", 1.5f);
            }
        }

        public void QueueVote()
        {
            if (popUpOptions.Length <= 1)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.mapListEmptyError, 1.5f);
                return;
            }
            if (MapHelper.isVoteInProgress) return; // return if map change is already queued

            Main.mapChangeManager.SendVoteInProgressEvent(true);

            ShowPopUpForAll(Labels.popUpMessage, true, PopupUtil.popUpTime);
            ShowMessageForAll(Labels.voteStartedMessage, 3.5f);
            StartCountdownForAll(PopupUtil.popUpTime);

            StartMapChangeRoutines(); // start routines after popups are not null
        }
        private IEnumerator ChangeMap()
        {
            yield return new WaitForSecondsRealtime(PopupUtil.popUpTime);

            try
            {
                if (MapHelper.isVoteInProgress)
                {
                    string votedMap = GetVotedMap();
                    LevelInfo mapInfo = MapHelper.GetMapInfo(votedMap);

                    if (!string.IsNullOrEmpty(votedMap) && mapInfo != null)
                    {
                        MapHelper.SetNextLevel(mapInfo);
                        ShowMessageForAll(Labels.voteCompleteMessage + votedMap, 1.5f);

                        //LevelManager.Instance.LoadLevel(mapInfo);
                        //LevelManager.Instance.PlayLevel(MapHelper.nextLevelInfo);
                        MultiplayerManager.Instance.LoadLevel(mapInfo);

                        ClearPopUpOptions();

                        MapHelper.Set_hasMapChangedByVote(true);
                    }
                    else
                    {
                        // Handle invalid map error
                        CancelVote(true);
                        MessageSystem.QueueMessage(MessageDisplayData.Type.Error, Labels.invalidMapError, 2.0f);
                        Main.Logger.Log(Labels.invalidMapError);
                    }
                }   
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Map Change Error - {ex.Message}");
            }
            finally
            {
                Main.mapChangeManager.SendVoteInProgressEvent(false);
            }

            yield return null;
        }

        public void CancelVote(bool overNetwork)
        {
            if (overNetwork)
            {
                CancelMapChangeOverNetwork();
                CancelMapChangeForSelf();
            }
            else
            {
                CancelMapChangeForSelf();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                Main.mapChangeManager.SendVoteInProgressEvent(false);
            }
            else
            {
                MapHelper.Set_isVoteInProgress(false);
            }
        }
        
        private void CancelMapChangeOverNetwork()
        {
            ShowPopUpForAll(Labels.voteCancelError, false, 0.1f);
            ShowMessageForAll(Labels.voteCancelError, 2.5f);
            StopCountdownForAll();
            ShowVoteListForAll(Array.Empty<Objective>());
        }
        private void CancelMapChangeForSelf()
        {
            ClearPopUpOptions();
            StopMapChangeRoutines();
            HideUI();
            ControlsHelper.EnableActiveControls(true);

            //MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"Voting cancelled", 1.5f); // remove later
        }
        private void HideUI()
        {
            ObjectiveListController.Instance.Hide();
            CountdownUI.Instance.StopCountdown();
            //MultiplayerManager.Instance.gameModePopup.gameObject.SetActive(false);
            PopupUtil.TimeoutPopup();
        }
       
        private void StartCountdownForAll(float time)
        {
            NetworkPlayerHelper.ForEachPlayerWithMod(player => player.ShowCountdown(time));
        }
        private void StopCountdownForAll()
        {
            NetworkPlayerHelper.ForEachPlayerWithMod(player => player.ShowCountdown(0.1f));
        }

        private void ShowMessageForAll(string message, float time)
        {
            NetworkPlayerHelper.ForEachPlayerWithMod(player => player.ShowMessage(message, time));
        }
        private void ShowVoteListForAll(Objective[] votelist)
        {
            NetworkPlayerHelper.ForEachPlayerWithMod(player => player.ShowObjectivesList(votelist));
        }
        private void ShowPopUpForAll(string message, bool pauseGame, float time)
        {
            HandlePopUpCallBack(); // initializes the call back and records the players answer.
            voteIndex = new Dictionary<int, int>(); // Re-initialize to make sure it's clean every time
            PopulateVoteIndex();

            NetworkPlayerHelper.ForEachPlayerWithMod(player => player.ShowPopup(message, popUpOptions, popUpCallBack, pauseGame, time));
        }
        private void HandlePopUpCallBack()
        {
            popUpCallBack = (optionIndex) => {
                //Handle the selected option here
                //Main.Logger.Log($"{GetOptionName(optionIndex)} Selected");
                LogPlayerChoice(optionIndex);
            };
        }
        private void LogPlayerChoice(int optionIndex)
        {
            if (voteIndex.ContainsKey(optionIndex))
            {
                voteIndex[optionIndex]++;
            }
        }
        public void AddMapToOptions(string selectedMap)
        {
            if (string.IsNullOrEmpty(selectedMap))
            {
                return;
            }
            // This means the map is not null, empty, and not found in the existing array
            var popUpOptionsList = new List<string>(popUpOptions);
            if (popUpOptionsList.Contains(selectedMap))
            {
                return;
            }
            else
            {
                popUpOptionsList.Add(selectedMap); // Add the new selection
                popUpOptions = popUpOptionsList.ToArray(); // Convert back to an array
            }
        }
        public void ClearPopUpOptions()
        {
            //popUpOptions = new string[] { };
            //voteIndex = new Dictionary<int, int>();
            popUpOptions = Array.Empty<string>();
            voteIndex.Clear();
        }
        private string GetVotedMap()
        {
            if (voteIndex.Count == 0)
            {
                return null; // No votes have been cast
            }

            int maxVotes = int.MinValue;
            List<int> tiedOptions = new List<int>();

            // Use a single pass to collect options with the maximum votes
            foreach (var pair in voteIndex)
            {
                if (pair.Value > maxVotes)
                {
                    // New maximum found, reset the list
                    maxVotes = pair.Value;
                    tiedOptions.Clear();
                    tiedOptions.Add(pair.Key);
                }
                else if (pair.Value == maxVotes)
                {
                    // Add to tied options list
                    tiedOptions.Add(pair.Key);
                }
            }

            // Return the option if thereis a winner
            if (tiedOptions.Count == 1 && popUpOptions.Length > 1)
            {
                int chosenIndex = tiedOptions[0];
                if (chosenIndex >= 0 && chosenIndex < popUpOptions.Length)
                {
                    return popUpOptions[chosenIndex];
                }
            }

            // Handle case that there is more than one winner
            ShowMessageForAll(Labels.tiedMapMessage, 2.5f);
            return MapHelper.ChooseMapOnTie(voteIndex, popUpOptions);
        }
        private IEnumerator UpdateVoteList()
        {
            float countdown = CountdownUtil.countdownDuration;

            while (countdown > 0f)
            {
                yield return new WaitForSecondsRealtime(1.0f);

                Objective[] votelist = ConvertToVoteList(GetVoteList());
                if (votelist == null)
                {
                    ShowVoteListForAll(Array.Empty<Objective>());
                    yield return null;
                }
                else
                {
                    ShowVoteListForAll(votelist);
                    yield return null;
                }
            }
            yield return null;
        }
        private void PopulateVoteIndex()
        {
            if (popUpOptions.Length <= 0)
                return;

            for (int i = 0; i < popUpOptions.Length; i++)
            {
                voteIndex[i] = 0; // Initialize all vote counts to 0
            }
        }
        private string[] GetVoteList()
        {
            if(voteIndex != null && voteIndex.Count > 0)
            {
                string[] votes = new string[voteIndex.Count];
                int i = 0;
                foreach (KeyValuePair<int, int> entry in voteIndex)
                {
                    // Ensure the key exists within the bounds of the popUpOptions array to avoid IndexOutOfRangeException
                    if (entry.Key >= 0 && entry.Key < popUpOptions.Length)
                    {
                        votes[i] = popUpOptions[entry.Key] + " : " + entry.Value; // Use the actual option text
                    }

                    i++;
                }
                return votes;
            }
            return null;
        }
        private Objective[] ConvertToVoteList(string[] votelist)
        {
            if (votelist != null && votelist.Length > 0)
            {
                Objective[] objectivelist = new Objective[votelist.Length];

                for (int i = 0; i < votelist.Length; i++)
                {
                    Objective objective = new Objective();
                    objective.name = votelist[i];
                    objectivelist[i] = objective;
                }
                return objectivelist;
            }
            return null;
        }
        
    }
}
