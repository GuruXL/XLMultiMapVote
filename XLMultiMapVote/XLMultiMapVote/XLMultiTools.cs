using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameManagement;
using ModIO.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Photon.Pun;

namespace XLMultiMapVote
{
    public class XLMultiMapVote : MonoBehaviour
    {
        private delegate void PlayerAction(NetworkPlayerController player);

        public Action<int> popUpCallBack;

        private string[] popUpOptions = new string[] { "Accept", "Decline" };

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
        public bool isLobbyMaster()
        {
            if (PhotonNetwork.IsMasterClient)
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

        public void ShowPlayerPopUp(string message, bool pauseGame, float time)
        {
            voteIndex.Clear();

            ForEachPlayer(player => player.ShowPopup(message, popUpOptions, popUpCallBack, pauseGame, time));

            popUpCallBack = (optionIndex) => {
                // Handle the selected option here
                Main.Logger.Log($"User selected popup option {optionIndex}");
                MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"User selected popup option {optionIndex}", 2.5f);
                LogPlayerChoice(optionIndex);
            };
        }
        public void LogPlayerChoice(int optionIndex)
        {
            if (!voteIndex.ContainsKey(optionIndex))
            {
                voteIndex[optionIndex] = 0;
            }
            else
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
    }
}
