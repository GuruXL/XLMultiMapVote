using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using XLMultiMapVote.Data;

namespace XLMultiMapVote.Map
{
    /*
    public class MapChangeManager2 : MonoBehaviourPunCallbacks
    {
        private const byte IsMapChangingEventCode = 233; // Use a unique code

        private void Awake()
        {
            LevelManager.Instance.OnLevelChanged += HandleLevelChanged;
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }
      
        private void OnDestroy()
        {
            LevelManager.Instance.OnLevelChanged -= HandleLevelChanged;
            PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
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
                    Main.multiMapVote.StopMapChangeRoutines();

                    PlayerController.Instance.respawn.ForceRespawn();

                    //MessageSystem.QueueMessage(MessageDisplayData.Type.Success, Labels.mapChangedMessage + levelInfo.name, 2.0f);
                    Main.Logger.Log(Labels.mapChangedMessage + levelInfo.name);

                    MapHelper.Set_hasMapChangedByVote(false);
                }
            }
        }
        public void OnEventReceivedold(EventData photonEvent)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (photonEvent.Code == IsMapChangingEventCode)
            {
                // Extract the data sent with the event
                object[] content = (object[])photonEvent.CustomData;
                bool isChanging = (bool)content[0];

                // Update your ismapchanging variable
                MapHelper.Set_isMapChanging(isChanging);
            }
        }
        public void OnEventReceived(EventData photonEvent)
        {
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    return;  // Make sure you understand why this check exists
            //}

            if (photonEvent.Code == IsMapChangingEventCode)
            {
                Main.Logger.Log("[OnEventReceived] Received IsMapChangingEvent. Processing...");

                object[] content = (object[])photonEvent.CustomData;
                if (content.Length > 0 && content[0] is bool isChanging)
                {
                    Main.Logger.Log($"[OnEventReceived] isChanging = {isChanging}");

                    MapHelper.Set_isMapChanging(isChanging);
                }
                else
                {
                    Main.Logger.Warning("[OnEventReceived] Received event but data was incorrect.");
                }
            }
        }
        IEnumerator SendMapChangeEvent(bool isMapChanging)
        {
            yield return new WaitForSeconds(1.0f); // Small delay

            if (PhotonNetwork.IsMasterClient)
            {
                object[] content = new object[] { isMapChanging }; // The data to send
                RaiseEventOptions options = new RaiseEventOptions()
                {
                    //Receivers = ReceiverGroup.Others, // Send to all other clients
                    Receivers = ReceiverGroup.All, // Send to all clients for testing
                    CachingOption = EventCaching.DoNotCache // Do not cache the event
                };
                SendOptions sendOptions = new SendOptions() { Reliability = true }; // ensure the event is delivered

                PhotonNetwork.RaiseEvent(IsMapChangingEventCode, content, options, sendOptions);

                // Update the host's ismapchanging variable
                MapHelper.Set_isMapChanging(isMapChanging);

                Main.Logger.Log($"[SendMapChangeEvent] Called isMapChanging : {MapHelper.isMapChanging} ");
            }
        }
        public void StartMapChange(bool isMapChanging)
        {
            StartCoroutine(SendMapChangeEvent(isMapChanging));
        }

        IEnumerator SendOnPlayerEnteredRoomEvent(Player newPlayer)
        {
            yield return new WaitForSeconds(1.0f); // Small delay for ensuring the new player is fully joined

            object[] content = new object[] { MapHelper.isMapChanging };
            RaiseEventOptions options = new RaiseEventOptions()
            {
                TargetActors = new int[] { newPlayer.ActorNumber }, // Send to the new player only
                CachingOption = EventCaching.DoNotCache
            };
            SendOptions sendOptions = new SendOptions() { Reliability = true };

            PhotonNetwork.RaiseEvent(IsMapChangingEventCode, content, options, sendOptions);

            Main.Logger.Log($"[OnPlayerEnteredRoom] Called isMapChanging : {MapHelper.isMapChanging} ");
        }

        public void SendOnPlayerEnterEvent(Player newPlayer)
        {
            StartCoroutine(SendOnPlayerEnteredRoomEvent(newPlayer));
        }

    }
    */
}