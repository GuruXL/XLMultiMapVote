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

        // UI Elements
        public Dropdown uiDropDownList;
        public Text mapLabelPrefab;

        private Button addMapButton;
        private Button ClearMapButton;
        private Button voteButton;
        private Button exitButton;

        private InputField filterMapsInput;
        private InputField timerInput;

        private void Awake()
        {
            StartCoroutine(CreateCustomUI());
        }

        private void Start()
        {
            GetButtonPrefab();
            CreateMenuButton();
            SetUpListeners();
        }

        private void OnDestroy()
        {
            DestroyMenuButton();
            RemoveListeners();
        }

        private IEnumerator CreateCustomUI()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);

            mapVoteUIobj = Instantiate(AssetLoader.MapVoteUIPrefab);
            mapVoteUIobj.transform.SetParent(Main.ScriptManager.transform);
            mapVoteUIobj.SetActive(false);
            Main.multiMapVote.voteState = mapVoteUIobj.AddComponent<VoteState>();
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

        private void NewButton(GameObject buttonPrefab, MenuButton customButton, string buttonName, UnityAction newListener, bool active)
        {
            if (customButton == null)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonPrefab.transform.parent);
                newButton.transform.SetSiblingIndex(buttonPrefab.transform.GetSiblingIndex() + 1); // adds new button one place below button prefab
                newButton.name = buttonName;

                customButton = newButton.GetComponent<MenuButton>();

                customButton.GreyedOut = false;
                customButton.GreyedOutInfoText = buttonName;
                customButton.Label.SetText(buttonName);
                customButton.interactable = true;

                customButton.onClick.RemoveAllListeners();  // Remove existing listeners
                customButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off); // removes persistant listeners that are set in unity editor.
                customButton.onClick.AddListener(newListener);  // Add new listener

                customButton.gameObject.SetActive(active);
            }
        }
        private void CreateMenuButton()
        {
            NewButton(menuButtonPrefab.gameObject, customMenuButton, "Vote For Map", MenuButtonOnClick, false);
        }
        public void DestroyMenuButton()
        {
            if (customMenuButton != null)
            {
                customMenuButton.onClick.RemoveAllListeners();
                Destroy(customMenuButton.gameObject);
                customMenuButton = null;
            }
        }
        private void MenuButtonOnClick()
        {
            GameStateMachine.Instance.RequestTransitionTo(Main.multiMapVote.voteState, true);
        }

        public void EnterVoteUI()
        {
            Time.timeScale = 0f;
            mapVoteUIobj.SetActive(true);
            GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);

            // Set the first selectable item in the Event System
            if (uiDropDownList != null)
            {
                EventSystem.current.SetSelectedGameObject(uiDropDownList.gameObject);
            }

            UpdateMapList();
        }
        public void ExitVoteUI()
        {
           mapVoteUIobj.SetActive(false);
        }
        
        private void GetUIComponents()
        {
            List<Button> uiButtons = new List<Button>();
            List<InputField> uiInputFields = new List<InputField>();

            // Get Buttons
            Button[] buttons = mapVoteUIobj.GetComponentsInChildren<Button>();
            uiButtons = buttons.ToList();
            addMapButton = GetButton(uiButtons, "Button_AddMap");
            ClearMapButton = GetButton(uiButtons, "Button_ClearMapList");
            voteButton = GetButton(uiButtons, "Button_Vote");
            exitButton = GetButton(uiButtons, "Button_Exit");

            // Get input fields
            InputField [] fields = mapVoteUIobj.GetComponentsInChildren<InputField>();
            uiInputFields = fields.ToList();
            filterMapsInput = GetInputField(uiInputFields, "InputField_FilterMap");
            timerInput = GetInputField(uiInputFields, "InputField_PopupTime");

            // Get Drop Down List for Maps
            uiDropDownList = mapVoteUIobj.GetComponentInChildren<Dropdown>();

            // Get Map Label Prefab For Map List Generaetion
            VerticalLayoutGroup mapListLayout;
            mapListLayout = mapVoteUIobj.GetComponentInChildren<VerticalLayoutGroup>();
            mapLabelPrefab = mapListLayout.GetComponentInChildren<Text>(true);
        }

        private Button GetButton(List<Button> buttonList, string name)
        {
            foreach (Button button in buttonList)
            {
                if (button.gameObject.name == name)
                {
                    return button;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        private InputField GetInputField(List<InputField> inputList, string name)
        {
            foreach (InputField field in inputList)
            {
                if (field.gameObject.name == name)
                {
                    return field;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        private void SetUpListeners()
        {
            addMapButton.onClick.AddListener(()=> AddMap());
            ClearMapButton.onClick.AddListener(() => ClearMapList());
            voteButton.onClick.AddListener(() => VoteButton());
            exitButton.onClick.AddListener(() => ExitButton());

            filterMapsInput.onEndEdit.AddListener(delegate { UpdateMapList(); });
            timerInput.onEndEdit.AddListener(delegate { UpdateTimerValue(); });
        }
        private void RemoveListeners()
        {
            addMapButton.onClick.RemoveAllListeners();
            ClearMapButton.onClick.RemoveAllListeners();
            voteButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();

            filterMapsInput.onEndEdit.RemoveAllListeners();
            timerInput.onEndEdit.RemoveAllListeners();
        }

        private void AddMap()
        {
            if (uiDropDownList.options.Count <= 0)
                return;

            Main.multiMapVote.AddMapToOptions(uiDropDownList.captionText.text);
        }

        private void ClearMapList()
        {
            Main.multiMapVote.ClearPopUpOptions();
            ClearDropDownList();

            // ** add function to destroy Created Map Label Objects **
        }

        private void ExitButton()
        {
            GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
        }

        private void VoteButton()
        {
            Main.multiMapVote.QueueVote();
        }

        private void PopulateDropDownList()
        {
            Dropdown.OptionDataList dropdownlist = new Dropdown.OptionDataList();
            string[] mapList = FilterMaps();

            for (int i = 0; i > mapList.Length; i++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = mapList[i];
                dropdownlist.options.Add(data);
            }

            uiDropDownList.options = dropdownlist.options;
        }

        private void ClearDropDownList()
        {
            uiDropDownList.ClearOptions();
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

        private void UpdateMapList()
        {
            ClearDropDownList();
            PopulateDropDownList();
        }

        private void CreateMapListLabel()
        {

        }
    }
}