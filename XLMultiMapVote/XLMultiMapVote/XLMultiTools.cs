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
            ForEachPlayer(player => player.ShowPopup(message, popUpOptions, popUpCallBack, pauseGame, time));
        }

        public void StartCountdown(float time)
        {
            ForEachPlayer(player => player.ShowCountdown(time));
        }

        public void ShowMessage(bool allPlayers, string message, float time)
        {
            ForEachPlayer(player => player.ShowMessage(message, time));
        }
    }
}
