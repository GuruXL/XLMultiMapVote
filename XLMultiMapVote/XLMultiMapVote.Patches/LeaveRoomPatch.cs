using HarmonyLib;
using XLMultiMapVote.Map;
using Photon.Pun;

namespace XLMultiMapVote.Patches
{
    [HarmonyPatch(typeof(MultiplayerManager))]
    [HarmonyPatch("LeaveRoom")]
    public static class LeaveRoomPatch
    {
        public static void Prefix()
        {
            if (MapHelper.isVoteInProgress)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Main.voteController.CancelVote(true);
                    Main.Logger.Log($"[LeaveRoomPatch] Vote cancelled over Network");
                }
                else
                {
                    Main.voteController.CancelVote(false);
                    Main.Logger.Log($"[LeaveRoomPatch] Vote cancelled locally");
                }
            }
        }
    }
}