using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameManagement;
using ModIO.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using TMPro;
using UnityEngine.Events;
using Rewired;
using UnityEngine.EventSystems;
using HarmonyLib;

namespace XLMultiMapVote.UI
{
    public class UIController : MonoBehaviour
    {
        //public bool isVoteUIactive { get; private set; } = false;

        public GameObject mapVoteUIobj;

        private GameObject menuButtonPrefab;
        public MenuButton customMenuButton;
        public MenuButton cancelVoteButton;
        private MultiplayerMainMenu.ButtonVisibilityDef menuButtonVisibility;
        private MultiplayerMainMenu.ButtonVisibilityDef cancelButtonVisibility;

        // UI Elements
        //public Dropdown uiDropDownList;
        public TMP_Dropdown uiDropDownList;
        //public Text mapLabelPrefab;
        public TextMeshProUGUI mapLabelPrefab;
        public List<TextMeshProUGUI> mapLabelList = new List<TextMeshProUGUI>();

        private Button addMapButton;
        private Button clearMapButton;
        private Button voteButton;
        private Button exitButton;

        //private InputField filterMapsInput;
        //private InputField timerInput;

        private TMP_InputField filterMapsInput;
        private TMP_InputField timerInput;

        private Canvas MapVoteListCanvas;
        private CanvasScaler voteListScaler;

        private Coroutine createCustomUIroutine;


        private void Awake()
        {
            CreateCustomUIRoutine();
        }

        private void Start()
        {
            SetUpMenuButtons();
        }

        private void OnDestroy()
        {
            DestroyButtons();
            RemoveListeners();
            Main.Logger.Log("UIController OnDestory Called");
        }

        private void CreateCustomUIRoutine()
        {
            if (createCustomUIroutine == null)
            {
                createCustomUIroutine = StartCoroutine(CreateCustomUI());
                Main.Logger.Log("CreateCustomUI Started");
            }
        }
        private IEnumerator CreateCustomUI()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);

            try
            {
                EventSystem.current.SetSelectedGameObject(null);

                mapVoteUIobj = Instantiate(AssetLoader.MapVoteUIPrefab);
                mapVoteUIobj.transform.SetParent(Main.ScriptManager.transform, false);
                //mapVoteUIobj.transform.SetParent(MultiplayerManager.Instance.menuController.transform, false);
                //mapVoteUIobj.transform.SetParent(CountdownUI.Instance.transform.parent, false);

                //EventSystem.current.SetSelectedGameObject(null);

                mapVoteUIobj.SetActive(false);

                Main.multiMapVote.voteState = mapVoteUIobj.AddComponent<VoteState>();

                GetUIComponents();
                AddUIComponents();
                SetUpListeners();

                SetUIInteractable(false);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during CreateCustomUI: " + ex.Message);
            }
            finally
            {
                // Reset the coroutine reference no matter what happens
                createCustomUIroutine = null;
            }
        }

        private void SetUpMenuButtons()
        {
            GetButtonPrefab();
            CreateMenuButton();
            CreateCancelVoteButton();
            //SetUpCanvasScaler();
        }
        
        private void GetButtonPrefab()
        {
            try
            {
                //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Players Button");
                //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Join Next Map Button");
                //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Start Game Mode");

                menuButtonPrefab = ButtonGetter.GetMultiplayerMenuButton();

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
            if (!Main.multiMapVote.isMapchanging)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"No Vote in Progress", 2.5f);
                return;
            }

            Main.multiMapVote.CancelVote(true);
        }
        public void EnterVoteUI()
        {
            Cursor.visible = true;

            EventSystem.current.SetSelectedGameObject(null);
            mapVoteUIobj.SetActive(true);
            SetUIInteractable(true);
            EventSystem.current.SetSelectedGameObject(uiDropDownList.gameObject);
            GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);

            ClearMapList(true);
            UpdateTimerValue();
        }

        public void ExitVoteUI()
        {
            Cursor.visible = false;

            EventSystem.current.SetSelectedGameObject(null);
            SetUIInteractable(false);
            mapVoteUIobj.SetActive(false);
        }

        private void GetUIComponents()
        {
            try
            {
                Transform addobj = mapVoteUIobj.transform.FindChildRecursively("Button_AddMap");
                addMapButton = addobj.GetComponent<Button>();

                Transform clearobj = mapVoteUIobj.transform.FindChildRecursively("Button_ClearMapList");
                clearMapButton = clearobj.GetComponent<Button>();

                Transform voteobj = mapVoteUIobj.transform.FindChildRecursively("Button_Vote");
                voteButton = voteobj.GetComponent<Button>();

                Transform exitobj = mapVoteUIobj.transform.FindChildRecursively("Button_Exit");
                exitButton = exitobj.GetComponent<Button>();

                Transform filterobj = mapVoteUIobj.transform.FindChildRecursively("InputField_FilterMap");
                //filterMapsInput = filterobj.GetComponent<InputField>();
                filterMapsInput = filterobj.GetComponent<TMP_InputField>();

                Transform timerobj = mapVoteUIobj.transform.FindChildRecursively("InputField_PopupTime");
                //timerInput = timerobj.GetComponent<InputField>();
                timerInput = timerobj.GetComponent<TMP_InputField>();

                Transform dropdownobj = mapVoteUIobj.transform.FindChildRecursively("MapListDropDown");
                //uiDropDownList = dropdownobj.GetComponent<Dropdown>();
                uiDropDownList = dropdownobj.GetComponent<TMP_Dropdown>();

                Transform maplabelobj = mapVoteUIobj.transform.FindChildRecursively("TextLabelPrefab");
                //mapLabelPrefab = maplabelobj.GetComponent<Text>();
                mapLabelPrefab = maplabelobj.GetComponent<TextMeshProUGUI>();

                MapVoteListCanvas = ObjectiveListController.Instance.gameObject.GetComponent<Canvas>();
                MapVoteListCanvas.sortingOrder = -1; // makes the UI appear behind the voting pop up instead of infront.

                voteListScaler = ObjectiveListController.Instance.gameObject.GetComponent<CanvasScaler>();
                voteListScaler.enabled = false;
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Getting Components - {ex.Message}");
            }     
        }
        private void AddUIComponents()
        {
            try
            {
                if (uiDropDownList != null)
                {
                    uiDropDownList.template.gameObject.AddComponent<ScrollRectAutoScroll>();
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Failed to add ScrollRectAutoScroll - {ex.Message}");
            }
        }
        private void SetUIInteractable(bool IsInteractable)
        {
            addMapButton.interactable = IsInteractable;
            clearMapButton.interactable = IsInteractable;
            voteButton.interactable = IsInteractable;
            exitButton.interactable = IsInteractable;
            filterMapsInput.interactable = IsInteractable;
            timerInput.interactable = IsInteractable;
            uiDropDownList.interactable = IsInteractable;
        }
        private void SetUpCanvasScaler()
        {
            voteListScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            voteListScaler.referenceResolution = new Vector2(3840, 2160);
            voteListScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            voteListScaler.matchWidthOrHeight = 0.5f;
            voteListScaler.referencePixelsPerUnit = 100f;
        }
        private void SetUpListeners()
        {
            addMapButton.onClick.AddListener(()=> AddMap());
            clearMapButton.onClick.AddListener(() => ClearMapList(true));
            voteButton.onClick.AddListener(() => VoteButton());
            exitButton.onClick.AddListener(() => ExitButton());

            //filterMapsInput.onEndEdit.AddListener(delegate { UpdateDropDownList(); });
            filterMapsInput.onValueChanged.AddListener(delegate { UpdateDropDownList(); });
            timerInput.onEndEdit.AddListener(delegate { UpdateTimerValue(); });

            //uiDropDownList.onValueChanged.AddListener(delegate { UpdateDropDownList(); });
        }
        private void RemoveListeners()
        {
            addMapButton.onClick.RemoveAllListeners();
            clearMapButton.onClick.RemoveAllListeners();
            voteButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();

            filterMapsInput.onEndEdit.RemoveAllListeners();
            timerInput.onEndEdit.RemoveAllListeners();

            //uiDropDownList.onValueChanged.RemoveAllListeners();
        }

        private void AddMap()
        {
            if (uiDropDownList.options.Count <= 0 
                || string.IsNullOrEmpty(uiDropDownList.captionText.text) 
                || uiDropDownList.captionText.text.Contains(Labels.addMapText))
                return;

            Main.multiMapVote.AddMapToOptions(uiDropDownList.captionText.text);
            CreateMapListLabel(uiDropDownList.captionText.text);
        }

        private void ClearMapList(bool clearpopUpOptions)
        {
            ClearMapLabels();
            UpdateDropDownList();

            if (clearpopUpOptions)
            {
                Main.multiMapVote.ClearPopUpOptions();
            }
        }
        private void ExitButton()
        {
            //GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
            GameStateMachine.Instance.RequestPlayState();
        }
        private void VoteButton()
        {
            if (uiDropDownList.options.Count <= 0 
                || Main.multiMapVote.popUpOptions.Length <= 0 
                || !MultiplayerManager.Instance.IsMasterClient)
                return;

            Main.multiMapVote.QueueVote();
            ClearMapList(false);
            GameStateMachine.Instance.RequestPlayState();
        }

        private void PopulateDropDownList()
        {
            List<TMP_Dropdown.OptionData> dropdownlist = new List<TMP_Dropdown.OptionData>();
            string[] mapList = MapHelper.FilterMaps(filterMapsInput.text);

            for (int i = 0; i < mapList.Length; i++)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = mapList[i];
                dropdownlist.Add(data); // Add the new option to the list
            }
            //uiDropDownList.options.AddRange(dropdownlist);
            uiDropDownList.AddOptions(dropdownlist);
        }
        private void UpdateDropDownList()
        {
            if (uiDropDownList == null)
            {
                Main.Logger.Error($"Error Updating Dropdown list - uiDropDownList is Null");
                return;
            }

            uiDropDownList.ClearOptions();
            SetDropDownCaption(Labels.addMapText);
            PopulateDropDownList();
        }
        private void SetDropDownCaption(string text)
        {
            uiDropDownList.captionText.text = text;
        }

        private void UpdateTimerValue()
        {
            float value = float.Parse(timerInput.text);

            if (Main.settings.popUpTime != value)
            {
                Main.settings.popUpTime = value;
            }
        }

        private void CreateMapListLabel(string mapName)
        {
            if (mapLabelList == null || mapLabelPrefab == null)
            {
                Main.Logger.Error($"Error Creating Map List Labels - Item is Null");
                return;
            }
            else if (string.IsNullOrEmpty(mapName) || mapName.Contains(Labels.addMapText))
            {
                return;
            }

            foreach (TextMeshProUGUI label in mapLabelList)
            {
                if (label.text == mapName)
                {
                    return; // skip creation if new label is a duplicate.
                }
            }

            GameObject newListObj = Instantiate(mapLabelPrefab.gameObject, mapLabelPrefab.gameObject.transform.parent);
            newListObj.SetActive(true);
            TextMeshProUGUI newListItem = newListObj.GetComponent<TextMeshProUGUI>();
            newListItem.text = mapName;
            newListItem.name = mapName;
            mapLabelList.Add(newListItem);
        }
        /* old version new on need for TMP
        private void CreateMapListLabel(string mapName)
        {
            if (string.IsNullOrEmpty(mapName) || mapName.Contains(Labels.addMapText))
            {
                return;
            }

            foreach (Text label in mapLabelList)
            {
                if (label.text == mapName)
                {
                    return; // skip creation if new label is a duplicate.
                }
            }

            GameObject newListObj = Instantiate(mapLabelPrefab.gameObject, mapLabelPrefab.gameObject.transform.parent);
            newListObj.SetActive(true);
            Text newListItem = newListObj.GetComponent<Text>();
            newListItem.text = mapName;
            newListItem.name = mapName;
            mapLabelList.Add(newListItem);
        }
        */

        private void ClearMapLabels()
        {
            StartCoroutine(DestroyLabelObjects());
        }
        private IEnumerator DestroyLabelObjects()
        {
            if (mapLabelList?.Count <= 0)
            {
                Main.Logger.Log($"Map Label List is Empty");
                yield break;
            }

            foreach (TextMeshProUGUI item in mapLabelList)
            {
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
                yield return new WaitForEndOfFrame();
            }

            mapLabelList.Clear();
        }

       
    }
}