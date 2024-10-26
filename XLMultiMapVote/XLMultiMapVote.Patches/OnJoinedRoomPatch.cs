using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using XLMultiMapVote.Map;

namespace XLMultiMapVote.Patches
{
    [HarmonyPatch(typeof(MultiplayerManager))]
    [HarmonyPatch("OnJoinedRoom")]
    public static class OnJoinedRoomPatch
    {
        public static void Postfix()
        {
            if (!PhotonNetwork.IsConnected)
            {
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                MapHelper.SetCurrentLevel(LevelManager.Instance.currentLevel);
            }
        }
    }
    
}