using UnityEngine;
using GameManagement;
using ModIO.UI;
using System;
using UnityEngine.Events;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using System.Linq;

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

            var firstActiveGO = MultiplayerManager.Instance.menuController.mainMenu.options
                .ElementAtOrDefault(7);

            return firstActiveGO?.buttonGO;
        }
        private void GetButtonPrefab()
        {
            try
            {
                //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Players Button");
                //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Join Next Map Button");
                //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Start Game Mode");

                menuButtonPrefab = GetMultiplayerMenuButton();

                if (menuButtonPrefab == null)
                {
                    Main.Logger.Error("menuButtonPrefab Not Found");
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Finding MainMenuButton Prefab - {ex.Message}");
            }
        }

        private void CreateMenuButton()
        {
            try
            {
                if (customMenuButton == null)
                {
                    GameObject newButton = Instantiate(menuButtonPrefab, menuButtonPrefab.transform.parent);
                    newButton.transform.SetSiblingIndex(menuButtonPrefab.gameObject.transform.GetSiblingIndex() - 1); // adds new button one place below button prefab
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
                Main.Logger.Error($"Failed to Create Vote Button - {ex.Message}");
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
                    cancelButtonVisibility.showOnlyForMasterClient = true;
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
                Main.Logger.Error($"Failed to Create Cancel Button - {ex.Message}");
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
            GameStateMachine.Instance.RequestTransitionTo(Main.multiMapVote.voteState, true);
        }
        private void CancelButtonOnClick()
        {
            if (!Main.multiMapVote.isMapChanging)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"No Vote in Progress", 2.5f);
                return;
            }

            Main.multiMapVote.CancelVote(true);
        }
    }
}
