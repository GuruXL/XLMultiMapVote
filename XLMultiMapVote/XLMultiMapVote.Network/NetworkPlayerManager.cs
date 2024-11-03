using ExitGames.Client.Photon;
using GameManagement;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using XLMultiMapVote.Data;
using XLMultiMapVote.Map;
using XLMultiMapVote.Utils;


namespace XLMultiMapVote.Network
{
   public class NetworkPlayerManager : MonoBehaviourPunCallbacks
   {
        private void Start()
        {
            NetworkPlayerHelper.SetPlayerProperties(Main.settings.isVotingEnabled); // initialze the IsVotingEnabled player property for later.
        }
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Main.settings.isVotingEnabled = true;
            }
            NetworkPlayerHelper.SetPlayerProperties(Main.settings.isVotingEnabled);
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (changedProps.ContainsKey(NetworkPlayerHelper.IsVoteEnabled))
            {
                bool isVotingEnabled = (bool)changedProps[NetworkPlayerHelper.IsVoteEnabled];
                Main.Logger.Log($"[OnPlayerPropertiesUpdate] Player {targetPlayer.NickName}: IsVotingEnabled updated to {isVotingEnabled}");
            }
        }
   }
}
