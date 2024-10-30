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
using HarmonyLib;
using MapEditor;
using Photon.Pun;

namespace XLMultiMapVote
{
    public class XLMultiMapVote : MonoBehaviour
    {
        private delegate void PlayerAction(NetworkPlayerController player);

        public Action<int> popUpCallBack;

        //public string[] popUpOptions = new string[] { "Option 0", "Option 1", "Option 2", "Option 3", };
        public string[] popUpOptions = new string[] { };

        public Dictionary<int, int> voteIndex = new Dictionary<int, int>();

        public VoteState voteState;

        private Coroutine changeMapCoroutine;
        private Coroutine updateVoteListCoroutine;
      
        public void StartMapChangeRoutines()
        {
            // Start the ChangeMap coroutine and store the reference
            if (changeMapCoroutine == null)
            {
                changeMapCoroutine = StartCoroutine(ChangeMap());

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "Change Map Started", 1.5f);
            }

            // Start the UpdateVoteList coroutine and store the reference
            if (updateVoteListCoroutine == null)
            {
                updateVoteListCoroutine = StartCoroutine(UpdateVoteList());

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "Update Vote List Started", 1.5f);
            }
        }
        public void StopMapChangeRoutines()
        {
            // Stop ChangeMap if it's running
            if (changeMapCoroutine != null)
            {
                StopCoroutine(changeMapCoroutine);
                changeMapCoroutine = null; // Set to null so it can be restarted again

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "Change Map Stopped", 1.5f);
            }

            // Stop UpdateVoteList if it's running
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

            //MapHelper.Set_isMapChanging(true);
            //Main.mapChangeManager.SendMapChangeEvent(true);
            Main.mapChangeManager.SendVoteInProgressEvent(true);

            ShowPlayerPopUp(Labels.popUpMessage, true, Main.settings.popUpTime);
            ShowMessage(Labels.voteStartedMessage, 3.5f);
            StartCountdown(Main.settings.popUpTime);

            StartMapChangeRoutines(); // start routines after pop ups for things are not null
        }
        public IEnumerator ChangeMap()
        {
            yield return new WaitForSecondsRealtime(Main.settings.popUpTime);

            try
            {
                if (MapHelper.isVoteInProgress)
                {
                    string votedMap = GetVotedMap();
                    LevelInfo mapInfo = MapHelper.GetMapInfo(votedMap);

                    if (!string.IsNullOrEmpty(votedMap) && mapInfo != null)
                    {
                        MapHelper.SetNextLevel(mapInfo);
                        ShowMessage(Labels.voteCompleteMessage + votedMap, 1.5f);

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
                //MapHelper.Set_isMapChanging(false);
                //Main.mapChangeManager.SendMapChangeEvent(false);
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
            ShowPlayerPopUp(Labels.voteCancelError, false, 1f);
            ShowMessage(Labels.voteCancelError, 3.0f);
            StopCountdown();
            ShowVoteList(Array.Empty<Objective>());
        }
        private void CancelMapChangeForSelf()
        {
            ClearPopUpOptions();
            StopMapChangeRoutines();

            HideUI();

            if (!InputController.Instance.controlsActive) // OnDisable is not being called on gameModePopup, This is Temp fix for frozen player controls
            {
                InputController.Instance.controlsActive = true;
            }

            //MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"Voting cancelled", 1.5f); // remove later
        }
        public void HideUI()
        {
            ObjectiveListController.Instance.Hide();
            CountdownUI.Instance.StopCountdown();
            MultiplayerManager.Instance.gameModePopup.gameObject.SetActive(false);
        }
        private void ForEachPlayer(PlayerAction action)
        {
            foreach (KeyValuePair<int, NetworkPlayerController> entry in MultiplayerManager.Instance.networkPlayers)
            {
                NetworkPlayerController player = entry.Value;
                if (player)
                {
                    action(player);
                }
            }
        }
        public void StartCountdown(float time)
        {
            ForEachPlayer(player => player.ShowCountdown(time));
        }
        public void StopCountdown()
        {
            ForEachPlayer(player => player.ShowCountdown(0.1f));
        }

        public void ShowMessage(string message, float time)
        {
            ForEachPlayer(player => player.ShowMessage(message, time));
        }

        public void ShowVoteList(Objective[] votelist)
        {
            ForEachPlayer(player => player.ShowObjectivesList(votelist));
        }
        public void ShowPlayerPopUp(string message, bool pauseGame, float time)
        {
            HandlePopUpCallBack(); // initializes the call back and records the players answer.

            voteIndex = new Dictionary<int, int>(); // Re-initialize to make sure it's clean every time
            PopulateVoteIndex();

            ForEachPlayer(player => player.ShowPopup(message, popUpOptions, popUpCallBack, pauseGame, time));
        }
        private void HandlePopUpCallBack()
        {
            popUpCallBack = (optionIndex) => {
                //Handle the selected option here
                //Main.Logger.Log($"{GetOptionName(optionIndex)} Selected");
                //MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"option {optionIndex} Selected", 2.5f);
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

        /* GetOptionName() not needed anymore
        public string GetOptionName(int index)
        {
            // Check if the index is within the bounds of the popUpOptions array
            if (index >= 0 && index < popUpOptions.Length)
            {
                return popUpOptions[index]; // Return the matching option
            }
            else
            {
                return "Option";
            }
        }
        */

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
            ShowMessage(Labels.tiedMapMessage, 2.5f);
            return MapHelper.ChooseMapOnTie(voteIndex, popUpOptions);
        }
        private IEnumerator UpdateVoteList()
        {
            float countdown = Traverse.Create(CountdownUI.Instance).Field("durationLeft").GetValue<float>();

            while (countdown > 0.0f)
            {
                yield return new WaitForSecondsRealtime(0.5f);

                Objective[] votelist = ConvertToVoteList(GetVoteList());
                if (votelist == null)
                {
                    ShowVoteList(Array.Empty<Objective>());
                    yield return null;
                }
                else
                {
                    ShowVoteList(votelist);
                    yield return null; // Allows the loop to be more responsive
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
        public string[] GetVoteList()
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
