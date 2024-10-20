﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameManagement;
using ModIO.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using SkaterXL.Data;
using Photon.Pun;
using Photon.Realtime;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using static RootMotion.FinalIK.InteractionObject;
using System.Threading;

namespace XLMultiMapVote
{
    public class XLMultiMapVote : MonoBehaviour
    {
        private delegate void PlayerAction(NetworkPlayerController player);

        public Action<int> popUpCallBack;

        //public string[] popUpOptions = new string[] { "Option 0", "Option 1", "Option 2", "Option 3", };
        public string[] popUpOptions = new string[] { };

        public Dictionary<int, int> voteIndex = new Dictionary<int, int>();

        public VoteState voteState = new VoteState();

        //public float countDownValue { get; private set; } = 0.0f;

        public bool isMapchanging { get; private set; } = false;

        private Coroutine changeMapCoroutine;
        private Coroutine updateVoteListCoroutine;

        public void StartMapChangeRoutines()
        {
            // Start the ChangeMap coroutine and store the reference
            if (changeMapCoroutine == null)
            {
                changeMapCoroutine = StartCoroutine(ChangeMap());

                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "Change Map Started", 2f);
            }

            // Start the UpdateVoteList coroutine and store the reference
            if (updateVoteListCoroutine == null)
            {
                updateVoteListCoroutine = StartCoroutine(UpdateVoteList());

                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, "update vote list Started", 2f);
            }
        }
        public void StopMapChangeRoutines()
        {
            // Stop ChangeMap if it's running
            if (changeMapCoroutine != null)
            {
                StopCoroutine(changeMapCoroutine);
                changeMapCoroutine = null; // Set to null so it can be restarted again

                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "Change Map Stopped", 2f);
            }

            // Stop UpdateVoteList if it's running
            if (updateVoteListCoroutine != null)
            {
                StopCoroutine(updateVoteListCoroutine);
                updateVoteListCoroutine = null; // Set to null so it can be restarted again

                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "Update Vote List Stopped", 2f);
            }

        }
        public void QueueVote()
        {
            if (popUpOptions.Length <= 1)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.mapListEmptyError, 1.5f);
                return;
            }
            if (isMapchanging) return;

            //StartCoroutine(ChangeMap());
            //StartCoroutine(UpdateVoteList());

            StartMapChangeRoutines();

            ShowPlayerPopUp(Labels.popUpMessage, true, Main.settings.popUpTime);
            ShowMessage(Labels.changeMapMessage, 5f);
            StartCountdown(Main.settings.popUpTime);

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
        public string[] GetVoteList()
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
        private void PopulateVoteIndex()
        {
            for (int i = 0; i < popUpOptions.Length; i++)
            {
                voteIndex[i] = 0; // Initialize all vote counts to 0
            }
        }

        public IEnumerator ChangeMap()
        {
            isMapchanging = true;

            yield return new WaitForSecondsRealtime(Main.settings.popUpTime);

            string votedMap = GetVotedMap();
            LevelInfo mapInfo = MapHelper.GetMapInfo(votedMap);

            if (!string.IsNullOrEmpty(votedMap) || mapInfo != null || !isMapchanging)
            {
                ShowMessage(Labels.changetoMessage + votedMap, 5f);
                //LevelManager.Instance.LoadLevel(MapHelper.GetMapInfo(votedMap));
                LevelManager.Instance.PlayLevel(MapHelper.GetMapInfo(votedMap));
                ClearPopUpOptions();
            }
            else
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, Labels.invalidMapError, 2.5f);
                Main.Logger.Log(Labels.invalidMapError);
            }

            isMapchanging = false;
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

            if (tiedOptions.Count == 1)
            {
                int chosenIndex = tiedOptions[0];
                if (chosenIndex >= 0 && chosenIndex < popUpOptions.Length)
                {
                    return popUpOptions[chosenIndex];
                }
            }

            // Handle the tie case by using the existing method
            return MapHelper.ChooseMapOnTie(voteIndex, popUpOptions);
        }
        /* GetVotedMap() old version
        private string GetVotedMap()
        {
            if (voteIndex.Count == 0)
            {
                return null;
            }

            // Find the highest vote count
            int maxVotes = voteIndex.Values.Max();
            // Find all options that received the maximum number of votes
            var votedOptions = voteIndex.Where(pair => pair.Value == maxVotes).Select(pair => pair.Key).ToList();

            // If there is only one option with the highest votes, return it
            if (votedOptions.Count == 1)
            {
                // Make sure the index is valid for mapOptions
                if (votedOptions[0] >= 0 && votedOptions[0] < popUpOptions.Length)
                {
                    return popUpOptions[votedOptions[0]];
                }
            }
            else if (votedOptions.Count > 1) // handle tied vote
            {
                return ChooseMapOnTie(voteIndex, popUpOptions);
            }

            return null;
        }
        */

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
        }

        private void CancelMapChangeOverNetwork()
        {
            Objective[] blanklist = new Objective[0];
            ShowVoteList(blanklist);
            StopCountdown();
            ShowMessage(Labels.voteCancelError, 5f);
            ShowPlayerPopUp("", false, -1);

        }
        private void CancelMapChangeForSelf()
        {
            StopMapChangeRoutines();
            CountdownUI.Instance.StopCountdown();
            ObjectiveListController.Instance.Hide();
            ResetPopupsLocally();

            isMapchanging = false;

            MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"Voting cancelled", 2.5f); // remove later
        }
        private void ResetPopupsLocally()
        {
            ClearPopUpOptions();
            MultiplayerManager.Instance.gameModePopup.gameObject.SetActive(false);
            if (!InputController.Instance.controlsActive)
            {
                InputController.Instance.controlsActive = true;
            }

            // Declare a blank callback action
            //Action<string, int> blankCallback = (string s, int i) => { };
            //string[] blankOptions = new string[1]
            //{
            //    "Close",
            //};
            //MultiplayerManager.Instance.gameModePopup.ShowPopup("blank", "", blankOptions, blankCallback, false);
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
        public void AddMapToOptions(string selectedMap)
        {
            if (string.IsNullOrEmpty(selectedMap) || selectedMap.Contains(Labels.addMapText))
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
            popUpOptions = new string[] { };
            voteIndex = new Dictionary<int, int>();
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

        private Objective[] ConvertVoteList()
        {
            string[] votelist = GetVoteList();

            Objective[] objectivelist = new Objective[votelist.Length];

            for (int i = 0; i < votelist.Length; i++)
            {
                Objective objective = new Objective();
                objective.name = votelist[i];
                objectivelist[i] = objective;
            }
            return objectivelist;
        }   
        private IEnumerator UpdateVoteList()
        {
            float countdown;
            yield return null;

            if (float.TryParse(CountdownUI.Instance.text.text, out countdown))
            {
                while (countdown > 0.0f)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    ShowVoteList(ConvertVoteList());
                    yield return null;
                }
            }
        }
    }
}
