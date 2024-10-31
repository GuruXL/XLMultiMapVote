using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using ModIO.UI;
using XLMultiMapVote.Map;

namespace XLMultiMapVote.Patches
{
    [HarmonyPatch(typeof(MultiplayerGameModePopup))]
    [HarmonyPatch("OnEnable")]
    public static class MultiplayerGameModePopupPatch
    {
        public static void Postfix()
        {
           
        }
    }
}