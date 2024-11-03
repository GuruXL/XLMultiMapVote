using GameManagement;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using XLMultiMapVote.Map;

namespace XLMultiMapVote.Patches
{
    /* only needed if custom player property is not working for filtering players with mod
    [HarmonyPatch(typeof(GameStateMachine))]
    [HarmonyPatch("ExitGame")]
    public static class ExitGamePatch
    {
        public static void Prefix()
        {
            if (PhotonNetwork.IsConnected 
                && PhotonNetwork.InRoom
                && PhotonNetwork.IsMasterClient
                && MapHelper.isVoteInProgress) 
            {
                Main.voteController.CancelVote(true);
            }
        }
    }
    */
}