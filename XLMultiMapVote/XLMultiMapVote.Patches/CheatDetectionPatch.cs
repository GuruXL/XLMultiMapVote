using HarmonyLib;
using Photon.Realtime;
using SkaterXL.Multiplayer;
using UnityEngine;

namespace XLMultiMapVote.Patches
{
    [HarmonyPatch(typeof(CheatDetection))]
    [HarmonyPatch("IsPlayerAllegibleToControl")]
    public static class CheatDetectionPatch
    {
        static bool Prefix(Player sender, Player controlledPlayer, CheatDetection.ControlType type, ref bool __result)
        {
            if (sender == null || controlledPlayer == null)
            {
                // Let the original method handle null cases
                return true;
            }

            if (type == CheatDetection.ControlType.Popup && Main.settings.isVotingEnabled)
            {
                __result = true; // Bypass Detection
                return false;    // Skip the original method
            }
            else
            {
                // allow the cheat detection to run as normal
                return true;
            }
        }
    }
}