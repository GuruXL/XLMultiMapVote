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

        public List<Text> mapLabelList = new List<Text>();

        private Button addMapButton;
        private Button clearMapButton;
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
                newButton.transform.SetAsFirstSibling();
                newButton.name = "Vote For Map";

                customMenuButton = newButton.GetComponent<MenuButton>();

                customMenuButton.GreyedOut = false;
                customMenuButton.GreyedOutInfoText = "Vote For Map";
                customMenuButton.Label.SetText("Vote For Map");
                customMenuButton.interactable = true;

                customMenuButton.onClick.RemoveAllListeners();  // Remove existing listeners
                customMenuButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off); // removes persistant listeners that are set in unity editor.
                customMenuButton.onClick.AddListener(() => MenuButtonOnClick());  // Add new listener

                customMenuButton.gameObject.SetActive(false);
            }
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
            //Time.timeScale = 0f;
            mapVoteUIobj.SetActive(true);
            GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);
            //EventSystem.current.SetSelectedGameObject(uiDropDownList.gameObject);
            UpdateMapList();
        }

        public void ExitVoteUI()
        {
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
            filterMapsInput = filterobj.GetComponent<InputField>();

            Transform timerobj = mapVoteUIobj.transform.FindChildRecursively("InputField_PopupTime");
            timerInput = timerobj.GetComponent<InputField>();

            Transform dropdownobj = mapVoteUIobj.transform.FindChildRecursively("MapListDropDown");
            uiDropDownList = dropdownobj.GetComponent<Dropdown>();

            Transform maplabelobj = mapVoteUIobj.transform.FindChildRecursively("TextLabelPrefab");
            mapLabelPrefab = maplabelobj.GetComponent<Text>();
        }
        private void AddUIComponents()
        {
            if (uiDropDownList != null)
            {
                uiDropDownList.template.gameObject.AddComponent<ScrollRectAutoScroll>();
            }
        }
        private void SetUpListeners()
        {
            addMapButton.onClick.AddListener(()=> AddMap());
            clearMapButton.onClick.AddListener(() => ClearMapList());
            voteButton.onClick.AddListener(() => VoteButton());
            exitButton.onClick.AddListener(() => ExitButton());

            //filterMapsInput.onEndEdit.AddListener(delegate { UpdateMapList(); });
            filterMapsInput.onValueChanged.AddListener(delegate { UpdateMapList(); });
            timerInput.onEndEdit.AddListener(delegate { UpdateTimerValue(); });

            //uiDropDownList.onValueChanged.AddListener(delegate { UpdateMapList(); });
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
            if (uiDropDownList.options.Count <= 0 || string.IsNullOrEmpty(uiDropDownList.captionText.text) || uiDropDownList.captionText.text.Contains(PopUpLabels.addMapText))
                return;

            Main.multiMapVote.AddMapToOptions(uiDropDownList.captionText.text);
            CreateMapListLabel(uiDropDownList.captionText.text);
        }

        private void ClearMapList()
        {
            Main.multiMapVote.ClearPopUpOptions();
            ClearMapLabels();
            SetDropDownCaption(PopUpLabels.addMapText);
        }

        private void ExitButton()
        {
            //GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
            GameStateMachine.Instance.RequestPlayState();
        }

        private void VoteButton()
        {
            if (uiDropDownList.options.Count <= 0 || Main.multiMapVote.popUpOptions.Length <= 0 || !MultiplayerManager.Instance.IsMasterClient)
                return;

            Main.multiMapVote.QueueVote();
            ClearDropDownList();
            ClearMapLabels();
            GameStateMachine.Instance.RequestPlayState();
        }

        private void PopulateDropDownList()
        {
            List<Dropdown.OptionData> dropdownlist = new List<Dropdown.OptionData>();
            string[] mapList = FilterMaps();

            for (int i = 0; i < mapList.Length; i++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = mapList[i];
                dropdownlist.Add(data); // Add the new option to the list
            }
            //uiDropDownList.options.AddRange(dropdownlist);
            uiDropDownList.AddOptions(dropdownlist);
        }

        private void ClearDropDownList()
        {
            uiDropDownList.ClearOptions();
            SetDropDownCaption(PopUpLabels.addMapText);
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

        private void UpdateMapList()
        {
            ClearDropDownList();
            PopulateDropDownList();
        }

        private void CreateMapListLabel(string mapName)
        {
            if (string.IsNullOrEmpty(mapName) || mapName.Contains(PopUpLabels.addMapText))
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

        private void ClearMapLabels()
        {
            StartCoroutine(DestroyLabelObjects());
        }
        private IEnumerator DestroyLabelObjects()
        {
            foreach (Text item in mapLabelList)
            {
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
                yield return new WaitForEndOfFrame();
            }

            mapLabelList.Clear();
        }

       
    }
}