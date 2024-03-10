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
        public delegate void PlayerAction(NetworkPlayerController player);

        public Action<int> popUpCallBack;

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

        public void ShowPlayerPopUp(string message)
        {
            string[] options = new string[] { "Accept", "Decline" };

            ForEachPlayer(player => player.ShowPopup(message, options, popUpCallBack, true, 10f));
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
