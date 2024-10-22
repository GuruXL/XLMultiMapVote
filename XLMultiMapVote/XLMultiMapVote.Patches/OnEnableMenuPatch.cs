using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using ModIO.UI;

namespace XLMultiMapVote.Patches
{
    
    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnEnable")]
    public static class OnEnableMenuPatch
    {
        private static MenuButton menuButton = Main.uiController.customMenuButton;
        private static MenuButton cancelButton = Main.uiController.cancelVoteButton;

        public static void Postfix(MultiplayerMainMenu __instance)
        {
            if (!PhotonNetwork.IsConnected)
                return;

            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            {
                if (menuButton.gameObject.activeSelf)
                {
                    menuButton.gameObject.SetActive(false);
                }
                if (cancelButton.gameObject.activeSelf)
                {
                    cancelButton.gameObject.SetActive(false);
                }
            }
            else if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null && PhotonNetwork.IsMasterClient)
            {
                menuButton.gameObject.SetActive(true);

                if (Main.multiMapVote.isMapchanging)
                {
                    cancelButton.gameObject.SetActive(true);
                }
                else 
                {
                    cancelButton.gameObject.SetActive(false);
                }
            }

            //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $" OnEnableMenu Patch has Run", 2.0f);
        }
    }

    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnLeftRoom")]
    public static class OnLeftRoomMenuPatch
    {
        private static MenuButton menuButton = Main.uiController.customMenuButton;
        private static MenuButton cancelButton = Main.uiController.cancelVoteButton;
        public static void Postfix(MultiplayerMainMenu __instance)
        {
            if (menuButton.gameObject.activeSelf)
            {
                menuButton.gameObject.SetActive(false);
            }
            if (cancelButton.gameObject.activeSelf)
            {
                cancelButton.gameObject.SetActive(false);
            }
        }
    }
}