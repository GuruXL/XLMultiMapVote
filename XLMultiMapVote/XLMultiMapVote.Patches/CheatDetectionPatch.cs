using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using ModIO.UI;

namespace XLMultiMapVote
{
    [HarmonyPatch(typeof(CheatDetection))]
    [HarmonyPatch("IsPlayerAllegibleToControl")]
    public static class CheatDetectionPatch
    {
        public static bool Prefix(ref bool __result, ref CheatDetection.ControlType controlType)
        {
            if (controlType != CheatDetection.ControlType.Popup)
            {
                return true; // continue original method
            }
            else
            {
                __result = true;
                Main.Logger.Log("Cheat Detection Bypassed");
                return false; // Skip the original method
            }
        }
    }
}