using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameManagement;
using ModIO.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using XLMultiMapVote.Data;

namespace XLMultiMapVote
{
    public class XLMultiMapVote : MonoBehaviour
    {
        private delegate void PlayerAction(NetworkPlayerController player);

        public Action<int> popUpCallBack;

        //public string[] popUpOptions = new string[] { "Option 0", "Option 1", "Option 2", "Option 3", };
        public string[] popUpOptions = new string[] {};

        public Dictionary<int, int> voteIndex = new Dictionary<int, int>();

        private void Awake()
        {
            MultiplayerManager.Instance.OnRoomJoined += OnJoined;
        }

        private void OnDestroy()
        {
            MultiplayerManager.Instance.OnRoomJoined -= OnJoined;
        }

        private void OnJoined()
        {

        }

        private void Update()
        {
           
        }
        public bool IsHost()
        {
            if (MultiplayerManager.Instance.IsMasterClient)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsInRoom()
        {
            if (MultiplayerManager.Instance.InRoom)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                // Handle the selected option here
                Main.Logger.Log($"option {optionIndex} Selected");
                MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"option {optionIndex} Selected", 2.5f);
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
        public void StartCountdown(float time)
        {
            ForEachPlayer(player => player.ShowCountdown(time));
        }

        public void ShowMessage(string message, float time)
        {
            ForEachPlayer(player => player.ShowMessage(message, time));
        }

        public string ChooseMapOnTie(Dictionary<int, int> voteIndex, string[] mapOptions)
        {
            // Find the highest vote count
            int maxVotes = voteIndex.Values.Max();

            // Find all options that received the maximum number of votes
            var tiedOptions = voteIndex.Where(pair => pair.Value == maxVotes).Select(pair => pair.Key).ToList();

            // Choose one of these options randomly if there's more than one
            if (tiedOptions.Count > 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, tiedOptions.Count); // Using Unity's Random for example
                int chosenOptionIndex = tiedOptions[randomIndex];
                // Inform players about the tie and the randomly selected option
                // Example: MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"Tie detected. Randomly selected {mapOptions[chosenOptionIndex]} as the next map.", 2.5f);
                return mapOptions[chosenOptionIndex];
            }
            else
            {
                // Only one option won outright
                return mapOptions[tiedOptions.First()];
            }
        }

        public void AddMapToOptions(string selectedMap)
        {
            if (string.IsNullOrEmpty(selectedMap) || selectedMap.Contains(PopUpLabels.addMapText))
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
            popUpOptions = new string[] {};
            voteIndex = new Dictionary<int, int>();
        }
    }
}
