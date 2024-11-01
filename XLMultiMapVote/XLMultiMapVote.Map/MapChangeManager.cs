using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;

namespace XLMultiMapVote.Map
{
    public class MapChangeManager : MonoBehaviourPunCallbacks
    {
        private const string IsVoteInProgress = "isVoteInProgress";

        private void Awake()
        {
            LevelManager.Instance.OnLevelChanged += HandleLevelChanged;
        }
        private void Start()
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    SetRoomProperties(false);  // Initialize to false on Master Client
            //}
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

                    //MessageSystem.QueueMessage(MessageDisplayData.Type.Success, Labels.mapChangedMessage + levelInfo.name, 2.0f);
                    Main.Logger.Log(Labels.mapChangedMessage + levelInfo.name);

                    MapHelper.Set_hasMapChangedByVote(false);
                }
            }
        }
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MapHelper.SetCurrentLevel(LevelManager.Instance.currentLevel, true);
                SendVoteInProgressEvent(false);
                Main.Logger.Log("[OnJoinedRoom] Joined room. Settings current level info as MasterClient");
            }
            else
            {
                GetMapChangingStateFromRoom(); // Fetch the current map-changing state from room properties
                Main.Logger.Log("[OnJoinedRoom] Joined room. Fetching current map changing state...");
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient && MapHelper.isVoteInProgress)
            {
                NetworkPlayerUtil.ForPlayer(newPlayer, player => player.ShowCountdown(CountdownUtil.countdownDuration));
                NetworkPlayerUtil.ForPlayer(newPlayer, player => player.ShowMessage(Labels.voteStartedMessage));
            }
        }
        public override void OnLeftRoom()
        {
            if (MapHelper.isVoteInProgress)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Main.voteController.CancelVote(true);
                    Main.Logger.Log($"[OnLeftRoom] Vote cancelled over Network");
                }
                else
                {
                    Main.voteController.CancelVote(false);
                    Main.Logger.Log($"[OnLeftRoom] Vote cancelled locally");
                }
            }
        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (MapHelper.isVoteInProgress && PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom != null)
            {
                if (newMasterClient.IsLocal)
                {
                    Main.voteController.CancelVote(true);
                }
                else
                {
                    Main.voteController.CancelVote(false);
                }
                Main.Logger.Log($"[OnMasterClientSwitched] new MasterClient : {newMasterClient.ActorNumber} : {newMasterClient.NickName}");
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
                    Main.Logger.Log($"[OnRoomPropertiesUpdate] Room property '{IsVoteInProgress}' updated to: {MapHelper.isVoteInProgress}");
                }
            }      
        }
        private void SetRoomProperties(bool isVoteInProgress)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Create a hashtable to hold the room property
                ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable{ { IsVoteInProgress,isVoteInProgress } };

                // Set the custom property for the room
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                Main.Logger.Log($"[SetRoomProperties] Room property '{IsVoteInProgress}' set to: {MapHelper.isVoteInProgress}");
            }
            else
            {
                Main.Logger.Warning("[SetRoomProperties] Only the Master Client can set the map changing state.");
            }
        }
        private void GetMapChangingStateFromRoom()
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(IsVoteInProgress))
            {
                bool isVoteInProgress = (bool)PhotonNetwork.CurrentRoom.CustomProperties[IsVoteInProgress];
                MapHelper.Set_isVoteInProgress(isVoteInProgress);
                Main.Logger.Log($"[GetMapChangingStateFromRoom] Read isMapChanging from room properties: {MapHelper.isVoteInProgress}");
            }
            else
            {
                Main.Logger.Warning("[GetMapChangingStateFromRoom] Room properties do not contain 'isMapChanging'.");
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