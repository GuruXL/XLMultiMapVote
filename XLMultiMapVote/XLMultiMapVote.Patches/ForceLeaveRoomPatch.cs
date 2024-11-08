﻿using HarmonyLib;
using XLMultiMapVote.Map;
using Photon.Pun;

namespace XLMultiMapVote.Patches
{
    [HarmonyPatch(typeof(MultiplayerManager))]
    [HarmonyPatch("ForceLeaveRoom")]
    public static class ForceLeaveRoomPatch
    {
        public static void Prefix()
        {
            if (MapHelper.isVoteInProgress)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Main.voteController.CancelVote(true);
                    //Main.Logger.Log($"[ForceLeaveRoom] Vote cancelled over Network");
                }
                else
                {
                    Main.voteController.CancelVote(false);
                    MapHelper.Set_isVoteInProgress(false);
                    //Main.Logger.Log($"[ForceLeaveRoom] Vote cancelled locally");
                }
            }
        }
    }

}