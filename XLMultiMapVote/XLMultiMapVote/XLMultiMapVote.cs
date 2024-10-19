using UnityEngine;
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
            //yield return new WaitForSeconds(Main.settings.popUpTime);
            yield return new WaitForSecondsRealtime(Main.settings.popUpTime);
            string votedMap = GetVotedMap();

            if (!string.IsNullOrEmpty(votedMap) || MapHelper.GetMapInfo(votedMap) != null)
            {
                ShowMessage(Labels.changetoMessage + votedMap, 5f);
                //LevelManager.Instance.LoadLevel(MapHelper.GetMapInfo(votedMap));
                LevelManager.Instance.PlayLevel(MapHelper.GetMapInfo(votedMap));
                ClearPopUpOptions();
                isMapchanging = false;
            }
            else
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, Labels.invalidMapError, 2.5f);
                Main.Logger.Log(Labels.invalidMapError);
                isMapchanging = false;
            }
        }

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
            else if (votedOptions.Count > 1) // A tie exists
            {
                return ChooseMapOnTie(voteIndex, popUpOptions);
            }

            return null;
        }
        public string ChooseMapOnTie(Dictionary<int, int> voteIndex, string[] mapOptions)
        {
            // This function assumes there is already a tie and doesn't check it by itself
            int maxVotes = voteIndex.Values.Max();
            var tiedOptions = voteIndex.Where(pair => pair.Value == maxVotes).Select(pair => pair.Key).ToList();

            int randomIndex = UnityEngine.Random.Range(0, tiedOptions.Count);
            int chosenOptionIndex = tiedOptions[randomIndex];

            return mapOptions[chosenOptionIndex];
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

        public void QueueVote()
        {
            if (isMapchanging || popUpOptions.Length <= 1)
                return;

            StartCoroutine(ChangeMap());

            ShowPlayerPopUp(Labels.popUpMessage, true, Main.settings.popUpTime);
            ShowMessage(Labels.changeMapMessage, 5f);
            StartCountdown(Main.settings.popUpTime);
            StartCoroutine(UpdateVoteList());
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
