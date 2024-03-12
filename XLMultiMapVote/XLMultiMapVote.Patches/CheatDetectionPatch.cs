using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;

namespace XLMultiMapVote.Patches
{
    [HarmonyPatch(typeof(CheatDetection))]
    [HarmonyPatch("IsPlayerAllegibleToControl")]
    public static class CheatDetectionPatch
    {
        public static bool Prefix(CheatDetection.ControlType type, ref bool __result)
        {
            if (type == CheatDetection.ControlType.Popup && Main.settings.allowPopUps)
            {
                __result = true; // Bypass cheat detection
                //Main.Logger.Log("Cheat Detection Bypassed");
                return false; // Skip the original method
            }
            return true; // continue with the original method
        }
    }
}