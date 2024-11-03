using Photon.Realtime;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace XLMultiMapVote.Network
{
    public static class NetworkPlayerHelper
    {
        private delegate void PlayerAction(NetworkPlayerController player);

        public const string IsVoteEnabled = "isVotingEnabled";
        public static bool IsVotingEnabled(Player player)
        {
            if (player.CustomProperties.TryGetValue(IsVoteEnabled, out object value))
            {
                if (value is bool votingEnabled)
                {
                    Main.Logger.Log($"[IsVotingEnabled] Player {player.NickName}: IsVotingEnabled = {votingEnabled}");
                    return votingEnabled;
                }
                else
                {
                    Main.Logger.Log($"[IsVotingEnabled] Player {player.NickName}: IsVotingEnabled property not a Bool value");
                    return false;
                }            
            }
            else
            {
                Main.Logger.Log($"[IsVotingEnabled] Player {player.NickName}: IsVotingEnabled property not found.");
                return false;
            }
        }
        public static void SetPlayerProperties(bool enabled)
        {
            ExitGames.Client.Photon.Hashtable modProperties = new ExitGames.Client.Photon.Hashtable
            {
                { IsVoteEnabled, enabled }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(modProperties);
            Main.Logger.Log($"[SetPlayerProperties] Local Player - IsVotingEnabled Set to :{enabled}.");
        }
        private static void ForEachPlayer(Action<NetworkPlayerController> action)
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
        public static void ForEachPlayerWithMod(Action<NetworkPlayerController> action)
        {
            foreach (KeyValuePair<int, NetworkPlayerController> entry in MultiplayerManager.Instance.networkPlayers)
            {
                NetworkPlayerController player = entry.Value;
                if (player)
                {
                    if (IsVotingEnabled(player.PhotonPlayer))
                    {
                        action(player);
                    }
                }
            }
        }
        public static NetworkPlayerController GetNetworkPlayerController(Player player)
        {
            NetworkPlayerController networkPlayerController;
            if (MultiplayerManager.Instance.networkPlayers.TryGetValue(player.ActorNumber, out networkPlayerController))
            {
                return networkPlayerController;
            }
            Main.Logger.Warning(" No Player with id: " + player.ActorNumber + " found");
            return null;
        }
        public static IEnumerator WaitForPhotonNetwork()
        {
            if (IsPhotonNetworkInTransition())
            {
                yield return new WaitWhile(() => IsPhotonNetworkInTransition());
            }
            yield return null;
        }
        private static bool IsPhotonNetworkInTransition()
        {
            switch (PhotonNetwork.NetworkClientState)
            {
                case ClientState.Authenticating:
                case ClientState.JoiningLobby:
                case ClientState.DisconnectingFromMasterServer:
                case ClientState.ConnectingToGameServer:
                case ClientState.Joining:
                case ClientState.Leaving:
                case ClientState.DisconnectingFromGameServer:
                case ClientState.ConnectingToMasterServer:
                case ClientState.Disconnecting:
                case ClientState.ConnectingToNameServer:
                case ClientState.DisconnectingFromNameServer:
                    return true;
            }
            return false;
        }
    }
}