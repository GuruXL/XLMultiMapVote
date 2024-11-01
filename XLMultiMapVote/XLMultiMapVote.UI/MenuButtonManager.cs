using UnityEngine;
using GameManagement;
using ModIO.UI;
using System;
using UnityEngine.Events;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using XLMultiMapVote.Map;
using System.Linq;
using Photon.Pun;
using HarmonyLib;
using System.Reflection;

namespace XLMultiMapVote.UI
{
    public class MenuButtonManager : MonoBehaviour
    {
        //public bool isVoteUIactive { get; private set; } = false;

        private GameObject menuButtonPrefab;
        public MenuButton customMenuButton;
        public MenuButton cancelVoteButton;
        private MultiplayerMainMenu.ButtonVisibilityDef menuButtonVisibility;
        private MultiplayerMainMenu.ButtonVisibilityDef cancelButtonVisibility;

        private void Start()
        {
            SetUpMenuButtons();
        }

        private void OnDestroy()
        {
            DestroyButtons();
        }

        private void SetUpMenuButtons()
        {
            GetButtonPrefab();
            CreateMenuButton();
            CreateCancelVoteButton();
            //SetUpCanvasScaler();
        }
        public GameObject GetMultiplayerMenuButton()
        {
            //var firstActiveGO = MultiplayerManager.Instance.menuController.mainMenu.options
            //.FirstOrDefault(go => go.buttonGO.GetComponent<MenuButton>());

            //var firstActiveGO = MultiplayerManager.Instance.menuController.mainMenu.options
            //.LastOrDefault(go => go.buttonGO.GetComponent<MenuButton>());

            var firstActiveGO = MultiplayerManager.Instance.menuController.mainMenu.options.ElementAtOrDefault(7);

            return firstActiveGO?.buttonGO;
        }
        private void GetButtonPrefab()
        {
            try
            {
                if (menuButtonPrefab == null)
                {
                    //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Players Button");
                    //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Join Next Map Button");
                    //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Start Game Mode");

                    menuButtonPrefab = GetMultiplayerMenuButton();
                }   
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Finding MainMenuButton Prefab - {ex.Message}");
            }
            finally
            {
                if (menuButtonPrefab != null)
                {
                    Main.Logger.Log("menuButtonPrefab has been found");
                }
                else
                {
                    Main.Logger.Error("menuButtonPrefab Not Found");
                }
            }
        }

        private void CreateMenuButton()
        {
            try
            {
                if (customMenuButton == null)
                {
                    GameObject newButton = Instantiate(menuButtonPrefab, menuButtonPrefab.transform.parent);
                    newButton.transform.SetSiblingIndex(menuButtonPrefab.gameObject.transform.GetSiblingIndex() - 1); // adds new button above button prefab
                    //newButton.transform.SetAsFirstSibling();
                    //newButton.transform.SetAsLastSibling();
                    newButton.name = Labels.menuButtonLabel;

                    customMenuButton = newButton.GetComponent<MenuButton>();

                    //customMenuButton.GreyedOut = false;
                    customMenuButton.GreyedOutInfoText = Labels.menuButtonLabel;
                    customMenuButton.Label.SetText(Labels.menuButtonLabel);
                    //customMenuButton.interactable = true;

                    customMenuButton.onClick.RemoveAllListeners();  // Remove existing listeners
                    customMenuButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off); // removes persistant listeners that are set in unity editor.
                    customMenuButton.onClick.AddListener(() => MenuButtonOnClick());  // Add new listener


                    menuButtonVisibility = new MultiplayerMainMenu.ButtonVisibilityDef();
                    menuButtonVisibility.buttonGO = customMenuButton.gameObject;
                    menuButtonVisibility.showOnlyForMasterClient = true;
                    menuButtonVisibility.showInLobby = false;
                    menuButtonVisibility.showInPublicRoom = true;
                    menuButtonVisibility.showInPrivateRoom = true;
                    menuButtonVisibility.showWhenDisconnected = false;
                    MultiplayerManager.Instance.menuController.mainMenu.options.Add(menuButtonVisibility);

                    //customMenuButton.gameObject.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Creating Vote Button - {ex.Message}");
            }
            finally
            {
                if (customMenuButton != null)
                {
                    Main.Logger.Log("Vote Button has been created or already exists");
                }
                else
                {
                    Main.Logger.Error("Failed to create Vote Button");
                }
            }

        }
        private void CreateCancelVoteButton()
        {
            try
            {
                if (cancelVoteButton == null)
                {
                    GameObject newButton = Instantiate(menuButtonPrefab, menuButtonPrefab.transform.parent);

                    if (customMenuButton != null)
                    {
                        newButton.transform.SetSiblingIndex(customMenuButton.transform.GetSiblingIndex() + 1);
                    }
                    else
                    {
                        newButton.transform.SetAsLastSibling();
                        //newButton.transform.SetAsFirstSibling();
                    }

                    newButton.name = Labels.cancelButtonLabel;

                    cancelVoteButton = newButton.GetComponent<MenuButton>();

                    //cancelVoteButton.GreyedOut = false;
                    cancelVoteButton.GreyedOutInfoText = Labels.cancelButtonLabel;
                    cancelVoteButton.Label.SetText(Labels.cancelButtonLabel);
                    //cancelVoteButton.interactable = true;

                    cancelVoteButton.onClick.RemoveAllListeners();  // Remove existing listeners
                    cancelVoteButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off); // removes persistant listeners that are set in unity editor.
                    cancelVoteButton.onClick.AddListener(() => CancelButtonOnClick());  // Add new listener

                    cancelButtonVisibility = new MultiplayerMainMenu.ButtonVisibilityDef();
                    cancelButtonVisibility.buttonGO = cancelVoteButton.gameObject;
                    cancelButtonVisibility.showOnlyForMasterClient = false;
                    cancelButtonVisibility.showInLobby = false;
                    cancelButtonVisibility.showInPublicRoom = true;
                    cancelButtonVisibility.showInPrivateRoom = true;
                    cancelButtonVisibility.showWhenDisconnected = false;
                    MultiplayerManager.Instance.menuController.mainMenu.options.Add(cancelButtonVisibility);

                    //cancelVoteButton.gameObject.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Creating Cancel Button - {ex.Message}");
            }
            finally
            {
                if (cancelVoteButton != null)
                {
                    Main.Logger.Log("Cancel Button has been created or already exists");
                }
                else
                {
                    Main.Logger.Error("Failed to create Cancel Button");
                }
            }

        }
        
        public void DestroyButtons()
        {
            try
            {
                if (customMenuButton != null)
                {
                    customMenuButton.gameObject.SetActive(false);
                    customMenuButton.onClick.RemoveAllListeners();
                    Destroy(customMenuButton.gameObject);
                    MultiplayerManager.Instance.menuController.mainMenu.options.Remove(menuButtonVisibility);
                    menuButtonVisibility = null;
                    customMenuButton = null;
                }

                if (cancelVoteButton != null)
                {
                    cancelVoteButton.gameObject.SetActive(false);
                    cancelVoteButton.onClick.RemoveAllListeners();
                    MultiplayerManager.Instance.menuController.mainMenu.options.Remove(cancelButtonVisibility);
                    Destroy(cancelVoteButton.gameObject);
                    cancelButtonVisibility = null;
                    cancelVoteButton = null;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Exception Destroying Menu Buttons - {ex.Message}");
            }
        }
        private void MenuButtonOnClick()
        {
            GameStateMachine.Instance.RequestTransitionTo(Main.voteController.voteState, true);
        }
        private void CancelButtonOnClick()
        {
            if (!MapHelper.isVoteInProgress)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.voteNotInProgressError, 2.0f);
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                //Main.multiMapVote.CancelVote(false);
                //AccessTools.Method(typeof(MultiplayerGameModePopup), "TimeOut").Invoke(MultiplayerManager.Instance.gameModePopup, null);
                PopupUtil.TimeoutPopup();
                ControlsHelper.EnableActiveControls(true);
            }
            else
            {
                Main.voteController.CancelVote(true);
            }

            GameStateMachine.Instance.RequestPlayState();
        }
    }
}
