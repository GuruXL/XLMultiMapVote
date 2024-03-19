using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;

namespace XLMultiMapVote.Patches
{
    /*
    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnEnable")]
    public static class MultiplayerMenuPatch
    {
        public static void Postfix(MultiplayerMainMenu __instance)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null && MultiplayerManager.Instance.IsMasterClient)
            {
                Main.uiController.customMenuButton.gameObject.SetActive(true);
            }

            else if (PhotonNetwork.InLobby && Main.uiController.customMenuButton.gameObject.activeSelf)
            {
                Main.uiController.customMenuButton.gameObject.SetActive(false);
            }
        }
    }
    */

    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnJoinedRoom")]
    public static class OnJoinedRoomPatch
    {
        public static void Postfix()
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null && MultiplayerManager.Instance.IsMasterClient)
            {
                if (!Main.uiController.customMenuButton.gameObject.activeSelf)
                {
                    Main.uiController.customMenuButton.gameObject.SetActive(true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnLeftRoom")]
    public static class OnLeftRoomPatch
    {
        public static void Postfix()
        {
            if (Main.uiController.customMenuButton.gameObject.activeSelf)
            {
                Main.uiController.customMenuButton.gameObject.SetActive(false);
            }
        }
    }
}