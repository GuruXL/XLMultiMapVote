using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using ModIO.UI;
using XLMultiMapVote.Map;
using GameManagement;
using XLMultiMapVote.Utils;

namespace XLMultiMapVote.Patches
{
    /// <summary>
    /// this fixes replay state crashing the game if the game is pasued and the game mode popup is enabled
    /// </summary>
    [HarmonyPatch(typeof(MultiplayerGameModePopup))]
    [HarmonyPatch("Update")]
    public static class MultiplayerGameModePopupPatch
    {
        public static bool Prefix()
        {
            if (MapHelper.isVoteInProgress)
            {
                if (PopupUtil.isPaused() && GameStateMachine.Instance.CurrentState is ReplayState)
                {
                    Time.timeScale = 1f;
                    // Skip the original Update method
                    return false;
                }
                else
                {
                    return true;
                }
            }
            // Proceed with the original Update method if no vote is in progress
            return true;
        }
    }

    [HarmonyPatch(typeof(MultiplayerGameModePopup))]
    [HarmonyPatch("OnEnable")]
    public static class OnEnableGameModePopupPatch
    {
        public static void Postfix()
        {
            Main.popupMenuManager.Set_isPopUpActive(true);

            if (PopupUtil.isPaused())
            {
                GameStateMachine.Instance.allowRespawn = false;
                GameStateMachine.Instance.allowPinMovement = false;
            }

            //Main.Logger.Log($"[OnEnableGameModePopupPatch] triggered");
        }
    }
    [HarmonyPatch(typeof(MultiplayerGameModePopup))]
    [HarmonyPatch("OnDisable")]
    public static class OnDisableGameModePopupPatch
    {
        public static void Postfix()
        {
            Main.popupMenuManager.Set_isPopUpActive(false);

            GameStateMachine.Instance.allowRespawn = true;
            GameStateMachine.Instance.allowPinMovement = true;
            ControlsHelper.EnableActiveControls(true);

            //Main.Logger.Log($"[OnDisableGameModePopupPatch] triggered");
        }
    }
}