using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using ModIO.UI;
using MapEditor;
using UnityEngine.Assertions.Must;
using GameManagement;
using XLMultiMapVote.Data;
using UnityEngine.EventSystems;

namespace XLMultiMapVote.Patches
{
    /*
    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnEnable")]
    public static class OnEnableMenuPatch
    {
        public static void Postfix(MultiplayerMainMenu __instance)
        {
            if (!PhotonNetwork.IsConnected)
            {
                return;
            }

            MenuButton menuButton = Main.uiController.customMenuButton;
            MenuButton cancelButton = Main.uiController.cancelVoteButton;

            if (menuButton == null || cancelButton == null)
            {
                return;
            }

            if (PhotonNetwork.InRoom)
            {
               
                //menuButton.interactable = true;
                //menuButton.GreyedOut = false;

                if (Main.multiMapVote.isMapchanging)
                {
                    //cancelButton.gameObject.SetActive(true);
                    cancelButton.interactable = true;
                    cancelButton.GreyedOut = false;
                }
                else
                {
                    //cancelButton.gameObject.SetActive(false);
                    cancelButton.interactable = false;
                    cancelButton.GreyedOut = true;

                    //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $" OnEnableMenu Patch has Run", 2.0f);
                }
            }
            else
            {
                //menuButton.interactable = false;
                //menuButton.GreyedOut = true;
                cancelButton.interactable = false;
                cancelButton.GreyedOut = true;

            }


        }
    }
    */

    /* OnEnableMenuPatch old patch
        [HarmonyPatch(typeof(MultiplayerMainMenu))]
        [HarmonyPatch("OnEnable")]
        public static class OnEnableMenuPatch
        {
            public static void Postfix(MultiplayerMainMenu __instance)
            {
                if (!PhotonNetwork.IsConnected && !PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
                {
                    return;
                }

                MenuButton menuButton = Main.uiController.customMenuButton;
                MenuButton cancelButton = Main.uiController.cancelVoteButton;

                if (menuButton == null || cancelButton == null)
                {
                    return;
                }

                if (!PhotonNetwork.IsMasterClient)
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
                else if (PhotonNetwork.IsMasterClient)
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
        */

    /*
    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnLeftRoom")]
    public static class OnLeftRoomMenuPatch
    {
        public static void Postfix(MultiplayerMainMenu __instance)
        {
            MenuButton menuButton = Main.uiController.customMenuButton;
            MenuButton cancelButton = Main.uiController.cancelVoteButton;
            if (menuButton == null || cancelButton == null)
            {
                return;
            }

            if (menuButton.gameObject.activeSelf)
            {
                menuButton.gameObject.SetActive(false);
                menuButton.interactable = false;
                menuButton.GreyedOut = true;
            }
            if (cancelButton.gameObject.activeSelf)
            {
                cancelButton.gameObject.SetActive(false);
                cancelButton.interactable = false;
                cancelButton.GreyedOut = true;
            }
        }
    }
    */

    /* OnLeftRoomMenuPatch old patch
    [HarmonyPatch(typeof(MultiplayerMainMenu))]
    [HarmonyPatch("OnLeftRoom")]
    public static class OnLeftRoomMenuPatch
    {
        public static void Postfix(MultiplayerMainMenu __instance)
        {
            MenuButton menuButton = Main.uiController.customMenuButton;
            MenuButton cancelButton = Main.uiController.cancelVoteButton;
            if (menuButton == null || cancelButton == null)
            {
                return;
            }

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
    */
}