using ExitGames.Client.Photon;
using GameManagement;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using XLMultiMapVote.Network;

namespace XLMultiMapVote.Map
{
    public class MapChangeManager : MonoBehaviourPunCallbacks
    {
        private const string IsVoteInProgress = "isVoteInProgress";

        private void Awake()
        {
            LevelManager.Instance.OnLevelChanged += HandleLevelChanged;
        }
        private void OnDestroy()
        {
            LevelManager.Instance.OnLevelChanged -= HandleLevelChanged;
        }
        private void HandleLevelChanged(LevelInfo levelInfo)
        {
            if (!PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
            {
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                MapHelper.SetCurrentLevel(levelInfo, false);

                if (MapHelper.HasMapChanged() && MapHelper.hasMapChangedByVote)
                {
                    Main.voteController.StopMapChangeRoutines();

                    PlayerController.Instance.respawn.ForceRespawn();
                    MapHelper.Set_hasMapChangedByVote(false);

                    Main.Logger.Log(Labels.mapChangedMessage + levelInfo.name);
                }
            }
        }
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MapHelper.SetCurrentLevel(LevelManager.Instance.currentLevel, true);
                SendVoteInProgressEvent(false);
                //Main.Logger.Log("[OnJoinedRoom] Joined room. Settings current level info as MasterClient");
            }
            else
            {
                GetVoteInProgressFromRoom();
                //Main.Logger.Log("[OnJoinedRoom] Joined room. Fetching current voting progress state...");
            }
           
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient && MapHelper.isVoteInProgress)
            {
                if (NetworkPlayerHelper.IsVotingEnabled(newPlayer))
                {
                    StartCoroutine(DelayedSend(newPlayer));
                    //Main.Logger.Log($"[OnPlayerEnteredRoom] {newPlayer.ActorNumber} : {newPlayer.NickName} entered room while vote in progress.");
                }
                else
                {
                    //Main.Logger.Log($"[OnPlayerEnteredRoom] {newPlayer.ActorNumber} : {newPlayer.NickName} does not have voting enabled.");
                }
            }
        }
        private IEnumerator DelayedSend(Player newPlayer)
        {
            yield return NetworkPlayerHelper.WaitForPhotonNetwork();
            yield return new WaitForSecondsRealtime(1);

            if (NetworkPlayerHelper.IsVotingEnabled(newPlayer))
            {
                NetworkPlayerController player = NetworkPlayerHelper.GetNetworkPlayerController(newPlayer);
                player.ShowCountdown(CountdownUtil.countdownDuration);
                player.ShowMessage(Labels.voteStartedMessage);
            }
            yield return null;
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            bool isLocal = newMasterClient.IsLocal;

            if (isLocal)
            {
                MapHelper.SetCurrentLevel(LevelManager.Instance.currentLevel, true);
            }

            if (MapHelper.isVoteInProgress)
            {
                Main.voteController.CancelVote(isLocal);
                //Main.Logger.Log($"[OnMasterClientSwitched] Vote Cancelled Over Network :{isLocal}");
            }
        }
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (propertiesThatChanged.ContainsKey(IsVoteInProgress))
                {
                    bool isVoteInProgress = (bool)propertiesThatChanged[IsVoteInProgress];
                    MapHelper.Set_isVoteInProgress(isVoteInProgress);
                    //Main.Logger.Log($"[OnRoomPropertiesUpdate] Room property '{IsVoteInProgress}' updated to: {MapHelper.isVoteInProgress}");
                }
            }      
        }
        private void SetRoomProperties(bool isVoteInProgress)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable{ { IsVoteInProgress,isVoteInProgress } };

                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                Main.Logger.Log($"[SetRoomProperties] Room property '{IsVoteInProgress}' set to: {MapHelper.isVoteInProgress}");
            }
            else
            {
                Main.Logger.Warning("[SetRoomProperties] Only the MasterClient can set the map changing state.");
            }
        }
        private void GetVoteInProgressFromRoom()
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(IsVoteInProgress))
            {
                bool isVoteInProgress = (bool)PhotonNetwork.CurrentRoom.CustomProperties[IsVoteInProgress];
                MapHelper.Set_isVoteInProgress(isVoteInProgress);
                Main.Logger.Log($"[GetMapChangingStateFromRoom] Room property isVoteInProgress: {MapHelper.isVoteInProgress}");
            }
            else
            {
                Main.Logger.Warning("[GetMapChangingStateFromRoom] Room properties do not contain 'isVoteInProgress'.");
            }
        }
        public void SendVoteInProgressEvent(bool isVoteInProgress)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MapHelper.Set_isVoteInProgress(isVoteInProgress);
                SetRoomProperties(isVoteInProgress);
                Main.Logger.Log($"[SendVoteInProgressEvent] isVoteInProgress : {MapHelper.isVoteInProgress}.");
            }
        }
    }
}