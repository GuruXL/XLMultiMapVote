using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using XLMultiMapVote.Map;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace XLMultiMapVote.Patches
{
    /*
    [HarmonyPatch(typeof(MultiplayerManager))]
    [HarmonyPatch("OnPlayerEnteredRoom")]
    public static class OnPlayerEnteredRoomPatch
    {
        public static void Postfix(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Main.mapChangeManager.SendOnPlayerEnterEvent(newPlayer);
            }
        }
    }
    */
}