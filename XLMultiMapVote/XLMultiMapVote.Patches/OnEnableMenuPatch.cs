using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using ModIO.UI;
using XLMultiMapVote.Map;

namespace XLMultiMapVote.Patches
{
    /*
    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnEnable")]
    public static class OnEnableMenuPatch
    {
        public static void Postfix()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            if (PhotonNetwork.InRoom && MapHelper.isVoteInProgress)
            {
                Main.menuButtonManager.cancelVoteButton.gameObject.SetActive(true);
            }
            else
            {
                Main.menuButtonManager.cancelVoteButton.gameObject.SetActive(false);
            }
        }
    }
    */
}