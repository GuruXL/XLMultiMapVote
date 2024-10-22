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

namespace XLMultiMapVote.UI
{
    public class UIController : MonoBehaviour
    {
        //public bool isVoteUIactive { get; private set; } = false;

        public GameObject mapVoteUIobj;

        private Transform menuButtonPrefab;
        public MenuButton customMenuButton;
        public MenuButton cancelVoteButton;

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


        private void Awake()
        {
            StartCoroutine(CreateCustomUI());
        }

        private void Start()
        {
            GetButtonPrefab();
            CreateMenuButton();
            CreateCancelVoteButton();
            //SetUpCanvasScaler();
        }

        private void OnDestroy()
        {
            DestroyButtons();
            RemoveListeners();
        }

        private IEnumerator CreateCustomUI()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);

            mapVoteUIobj = Instantiate(AssetLoader.MapVoteUIPrefab);
            mapVoteUIobj.transform.SetParent(Main.ScriptManager.transform);
            mapVoteUIobj.SetActive(false);
            Main.multiMapVote.voteState = mapVoteUIobj.AddComponent<VoteState>();

            GetUIComponents();
            AddUIComponents();
            SetUpListeners();
        }

        private void GetButtonPrefab()
        {
            //menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Players Button");
            menuButtonPrefab = MultiplayerManager.Instance.menuController.mainMenu.transform.FindChildRecursively("Join Next Map Button");

            if (menuButtonPrefab != null)
            {
                Main.Logger.Log("MainMenuButton Prefab Found");
            }
            else
            {
                Main.Logger.Log("MainMenuButton Prefab NOT Found");
            }
        }

        private void CreateMenuButton()
        {
            if (customMenuButton == null)
            {
                GameObject newButton = Instantiate(menuButtonPrefab.gameObject, menuButtonPrefab.transform.parent);
                //newButton.transform.SetSiblingIndex(menuButtonPrefab.gameObject.transform.GetSiblingIndex() + 1); // adds new button one place below button prefab
                //newButton.transform.SetAsFirstSibling();
                newButton.transform.SetSiblingIndex(1);
                newButton.name = Labels.menuButtonLabel;

                customMenuButton = newButton.GetComponent<MenuButton>();

                customMenuButton.GreyedOut = false;
                customMenuButton.GreyedOutInfoText = Labels.menuButtonLabel;
                customMenuButton.Label.SetText(Labels.menuButtonLabel);
                customMenuButton.interactable = true;

                customMenuButton.onClick.RemoveAllListeners();  // Remove existing listeners
                customMenuButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off); // removes persistant listeners that are set in unity editor.
                customMenuButton.onClick.AddListener(() => MenuButtonOnClick());  // Add new listener

                customMenuButton.gameObject.SetActive(false);
            }
        }
        private void CreateCancelVoteButton()
        {
            if (cancelVoteButton == null)
            {
                GameObject newButton = Instantiate(menuButtonPrefab.gameObject, menuButtonPrefab.transform.parent);

                if (customMenuButton != null)
                {
                    newButton.transform.SetSiblingIndex(customMenuButton.transform.GetSiblingIndex() + 1);
                }
                else
                {
                    newButton.transform.SetSiblingIndex(2);
                }

                newButton.name = Labels.cancelButtonLabel;

                cancelVoteButton = newButton.GetComponent<MenuButton>();

                cancelVoteButton.GreyedOut = false;
                cancelVoteButton.GreyedOutInfoText = Labels.cancelButtonLabel;
                cancelVoteButton.Label.SetText(Labels.cancelButtonLabel);
                cancelVoteButton.interactable = true;

                cancelVoteButton.onClick.RemoveAllListeners();  // Remove existing listeners
                cancelVoteButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off); // removes persistant listeners that are set in unity editor.
                cancelVoteButton.onClick.AddListener(() => CancelButtonOnClick());  // Add new listener

                cancelVoteButton.gameObject.SetActive(false);
            }
        }
        public void DestroyButtons()
        {
            if (customMenuButton != null)
            {
                customMenuButton.onClick.RemoveAllListeners();
                Destroy(customMenuButton.gameObject);
                customMenuButton = null;
            }

            if (cancelVoteButton != null)
            {
                cancelVoteButton.onClick.RemoveAllListeners();
                Destroy(cancelVoteButton.gameObject);
                cancelVoteButton = null;
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
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"No Vote in Progress", 2.5f); // remove later
                return;
            }

            Main.multiMapVote.CancelVote(true);
        }
        public void EnterVoteUI()
        {
            Cursor.visible = true;

            //Time.timeScale = 0f;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.firstSelectedGameObject = uiDropDownList.gameObject;
            EventSystem.current.SetSelectedGameObject(uiDropDownList.gameObject);

            mapVoteUIobj.SetActive(true);
            GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);
            ClearMapList(true);
            UpdateTimerValue();
        }

        public void ExitVoteUI()
        {
            Cursor.visible = false;

            EventSystem.current.SetSelectedGameObject(null);

            mapVoteUIobj.SetActive(false);
        }

        private void GetUIComponents()
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
            //ObjectiveListController.Instance.gameObject.SetActive(true);

            voteListScaler = ObjectiveListController.Instance.gameObject.GetComponent<CanvasScaler>();
            voteListScaler.enabled = false;
        }
        private void AddUIComponents()
        {
            if (uiDropDownList != null)
            {
                uiDropDownList.template.gameObject.AddComponent<ScrollRectAutoScroll>();
            }

            bool componentadded = uiDropDownList.template.gameObject.GetComponent<ScrollRectAutoScroll>();
            if (componentadded)
            {
                Main.Logger.Log("ScrollRectAutoScroll Added");
            }
            else
            {
                Main.Logger.Log("Failed to add ScrollRectAutoScroll");
            }
            //voteListScaler = ObjectiveListController.Instance.gameObject.AddComponent<CanvasScaler>();
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
            string[] mapList = FilterMaps();

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
            uiDropDownList.ClearOptions();
            SetDropDownCaption(Labels.addMapText);
            PopulateDropDownList();
        }

        private void SetDropDownCaption(string text)
        {
            uiDropDownList.captionText.text = text;
        }

        private string[] FilterMaps()
        {
            string[] mapList;

            if (!string.IsNullOrEmpty(filterMapsInput.text))
            {
                mapList = MapHelper.FilterArray(MapHelper.GetMapNames(), filterMapsInput.text);
            }
            else
            {
                mapList = MapHelper.GetMapNames();
            }

            return mapList;
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
            if (string.IsNullOrEmpty(mapName) || mapName.Contains(Labels.addMapText))
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