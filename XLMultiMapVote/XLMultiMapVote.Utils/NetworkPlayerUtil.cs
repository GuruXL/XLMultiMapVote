using System.Reflection;
using System.IO;
using SkaterXL.Multiplayer;
using Photon.Realtime;
using System.Collections.Generic;
using System;

namespace XLMultiMapVote.Utils
{
    public static class NetworkPlayerUtil
    {
        private delegate void PlayerAction(NetworkPlayerController player);

        public static void ForEachPlayer(Action<NetworkPlayerController> action)
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

        public static void ForPlayer(Player photonPlayer, Action<NetworkPlayerController> action)
        {
            foreach (KeyValuePair<int, NetworkPlayerController> entry in MultiplayerManager.Instance.networkPlayers)
            {
                NetworkPlayerController player = entry.Value;
                if (player.PhotonPlayer == photonPlayer)
                {
                    action(player);
                }
            }
        }
    }
}