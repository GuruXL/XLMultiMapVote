using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameManagement;
using System;
using UnityEngine.UI;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using XLMultiMapVote.Map;
using XLMultiMapVote.State;
using XLMultiMapVote.UI.Components;
using TMPro;
using UnityEngine.EventSystems;
using ModIO.UI;

namespace XLMultiMapVote.UI
{
    public class UIController : MonoBehaviour
    {
        //public bool isVoteUIactive { get; private set; } = false;

        public GameObject mapVoteUIobj;

        // UI Elements
        public TMP_Dropdown uiDropDownList{ get; private set; }
        private Scrollbar uiDropDownScrollBar;
        public TextMeshProUGUI mapLabelPrefab;
        private Scrollbar mapLabelScrollBar;
        public List<TextMeshProUGUI> mapLabelList = new List<TextMeshProUGUI>();

        private Button addMapButton;
        private Button clearMapButton;
        private Button voteButton;
        private Button exitButton;

        public TMP_InputField filterMapsInput{ get; private set; }
        public TMP_InputField timerInput { get; private set; }

        private Canvas MapVoteListCanvas;
        private CanvasScaler voteListScaler;

        private Coroutine createCustomUIroutine;


        private void Awake()
        {
            CreateCustomUIRoutine();
        }

        private void OnDestroy()
        {
            RemoveListeners();
            //Main.Logger.Log("UIController OnDestory Called");
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

                Main.voteController.voteState = mapVoteUIobj.AddComponent<VoteState>();

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
 
        public void EnterVoteUI()
        {
            Cursor.visible = true;

            EventSystem.current.SetSelectedGameObject(null);

            mapVoteUIobj.SetActive(true);
            SetUIInteractable(true);
            EventSystem.current.SetSelectedGameObject(uiDropDownList.gameObject);
            GameStateMachine.Instance.SemiTransparentLayer.SetActive(false); 
            UpdateTimerValue();
            ClearMapList(true);
        }

        public void ExitVoteUI()
        {
            Cursor.visible = false;

            EventSystem.current.SetSelectedGameObject(null);

            SetUIInteractable(false);
            mapVoteUIobj.SetActive(false);
            UISounds.Instance.PlayOneShotExit();
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
                filterMapsInput = filterobj.GetComponent<TMP_InputField>();

                Transform timerobj = mapVoteUIobj.transform.FindChildRecursively("InputField_PopupTime");
                timerInput = timerobj.GetComponent<TMP_InputField>();

                Transform dropdownobj = mapVoteUIobj.transform.FindChildRecursively("MapListDropDown");
                uiDropDownList = dropdownobj.GetComponent<TMP_Dropdown>();
                uiDropDownScrollBar = uiDropDownList.template.GetComponentInChildren<Scrollbar>();

                Transform mapListScrollView = mapVoteUIobj.transform.FindChildRecursively("MapListScollView");
                mapLabelScrollBar = mapListScrollView.GetComponentInChildren<Scrollbar>();
                Transform maplabelobj = mapListScrollView.transform.FindChildRecursively("TextLabelPrefab");
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
                uiDropDownList.template.gameObject.AddComponent<ScrollRectAutoScroll>();

                uiDropDownList.gameObject.AddComponent<SelectableSounds>();
                uiDropDownList.itemText.transform.parent.gameObject.AddComponent<SelectableSounds>();
                uiDropDownScrollBar.gameObject.AddComponent<SelectableSounds>();

                mapLabelScrollBar.gameObject.AddComponent<SelectableSounds>();
                addMapButton.gameObject.AddComponent<SelectableSounds>();
                clearMapButton.gameObject.AddComponent<SelectableSounds>();
                voteButton.gameObject.AddComponent<SelectableSounds>();
                exitButton.gameObject.AddComponent<SelectableSounds>();
                filterMapsInput.gameObject.AddComponent<SelectableSounds>();
                timerInput.gameObject.AddComponent<SelectableSounds>();

                filterMapsInput.gameObject.AddComponent<DisableNavigationOnFocus>();
                timerInput.gameObject.AddComponent<DisableNavigationOnFocus>();
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Adding Components - {ex.Message}");
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
        /* SetUpCanvasScaler not needed
        private void SetUpCanvasScaler()
        {
            voteListScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            voteListScaler.referenceResolution = new Vector2(3840, 2160);
            voteListScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            voteListScaler.matchWidthOrHeight = 0.5f;
            voteListScaler.referencePixelsPerUnit = 100f;
        }
        */
        private void SetUpListeners()
        {
            addMapButton.onClick.AddListener(() => AddMap());
            clearMapButton.onClick.AddListener(() => ClearMapList(true));
            voteButton.onClick.AddListener(() => VoteButton());
            exitButton.onClick.AddListener(() => ExitButton());
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
            if (uiDropDownList.options.Count <= 0)
            {
                return;
            }
            string dropDownSelection = uiDropDownList.captionText.text;

            if (string.IsNullOrEmpty(dropDownSelection) 
                || dropDownSelection.Contains(MapHelper.currentLevelInfo.name) 
                || dropDownSelection.Contains(Labels.addMapText))
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.addMapError, 2.0f);
                return;
            }
            Main.voteController.AddMapToOptions(dropDownSelection);
            CreateMapListLabel(dropDownSelection);
        }

        private void ClearMapList(bool clearpopUpOptions)
        {
            ClearMapLabels();
            UpdateDropDownList();

            if (clearpopUpOptions)
            {
                Main.voteController.ClearPopUpOptions();
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
                || Main.voteController.popUpOptions.Length <= 0
                || !MultiplayerManager.Instance.IsMasterClient)
            {
                return;
            }             

            Main.voteController.QueueVote();
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
            float value = 30f; // Default value
            if (!string.IsNullOrEmpty(timerInput.text))
            {
                if (float.TryParse(timerInput.text, out float parsedValue))
                {
                    if (parsedValue >= 1f)
                    {
                        value = parsedValue;
                    }
                    else
                    {
                        value = 1f;
                        timerInput.SetTextWithoutNotify("1");
                    }
                }
                else
                {
                    timerInput.SetTextWithoutNotify("30");
                }
            }
            else
            {
                // If the text is null or empty, set it to "30"
                timerInput.SetTextWithoutNotify("30");
            }

            // Update the pop-up time if it has changed
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
        private void ClearMapLabels()
        {
            if (mapLabelList?.Count <= 0)
            {
                //Main.Logger.Warning($"Map Label List is Empty");
                return;
            }
            else
            {
                StartCoroutine(DestroyLabelObjects());
            }
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