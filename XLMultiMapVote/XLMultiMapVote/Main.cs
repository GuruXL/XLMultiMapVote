using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;
using ModIO.UI;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using XLMultiMapVote.UI;
using XLMultiMapVote.Map;
using XLMultiMapVote.Network;
using Photon.Pun;
using System;
using Object = UnityEngine.Object;

namespace XLMultiMapVote
{
    [EnableReloading]
    internal static class Main
    {
        public static bool enabled;
        public static Harmony harmonyInstance;
        public static string modId = "XLMultiMapVote";
        public static UnityModManager.ModEntry modEntry;
        public static Settings settings;
        public static GameObject ScriptManager;
        public static VoteController voteController;
        public static MapChangeManager mapChangeManager;
        public static NetworkPlayerManager networkPlayerManager;
        public static MenuButtonManager menuButtonManager;
        public static PopupMenuManager popupMenuManager;
        public static UIController uiController;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(OnSaveGUI);
                modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
                modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Unload);
                Main.modEntry = modEntry;
                Logger.Log(nameof(Load));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error Loading {modEntry}: {ex.Message}");
                return false;
            }

            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
                return;

            //GUILayout.BeginVertical("Box"); // Main Box

            GUILayout.BeginHorizontal();
            //settings.isVotingEnabled = GUILayout.Toggle(settings.isVotingEnabled, "Enable Voting", GUILayout.Width(128));
            RGUI.BeginBackgroundColor(GUIExtensions.ColorSwitch(settings.isVotingEnabled, Color.cyan, Color.white));
            if (GUILayout.Button(GUIExtensions.LabelSwitch("Voting Enabled", Color.white, "Voting Disabled", Color.gray, settings.isVotingEnabled), RGUIStyle.button, GUILayout.Width(128)))
            {     
                if (PhotonNetwork.IsMasterClient)
                {
                    if (!settings.isVotingEnabled)
                    {
                        settings.isVotingEnabled = true;
                        NetworkPlayerHelper.SetPlayerProperties(settings.isVotingEnabled);            
                    }
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.disableVoteAsHostError, 2.0f);
                    return;
                }
                else
                {
                    settings.isVotingEnabled = !settings.isVotingEnabled;
                    NetworkPlayerHelper.SetPlayerProperties(settings.isVotingEnabled);
                }
            }
            RGUI.EndBackgroundColor();
            RGUI.BeginBackgroundColor(Color.white);
            if (GUILayout.Button("Cancel Vote", RGUIStyle.button, GUILayout.Width(128)))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    voteController.CancelVote(true);
                }
                else
                {
                    voteController.CancelVote(false);
                }
            }
            RGUI.EndBackgroundColor();
            GUILayout.EndHorizontal();

            /*
            GUILayout.Space(6);

            if (MultiplayerManager.Instance.IsMasterClient)
            {
                GUILayout.Label("Options");
                GUILayout.BeginHorizontal();
                RGUI.BeginBackgroundColor(Color.cyan);
                if (GUILayout.Button("Queue Vote", RGUIStyle.button, GUILayout.Width(128)))
                {
                    multiMapVote.StartCoroutine(multiMapVote.ChangeMap());

                    multiMapVote.ShowPlayerPopUp(PopUpLabels.popUpMessage, true, settings.popUpTime);
                    multiMapVote.ShowMessage(PopUpLabels.changeMapMessage, 5f);
                    multiMapVote.StartCountdown(settings.popUpTime);
                }
                RGUI.EndBackgroundColor();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                RGUI.BeginBackgroundColor(Color.cyan);
                settings.popUpTime = RGUI.Field(Mathf.RoundToInt(settings.popUpTime), "Time: ", GUILayout.MaxWidth(96));
                RGUI.EndBackgroundColor();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                string voteResults = string.Join("\n", multiMapVote.GetVoteList());
                GUILayout.Label($"Votes: \n{voteResults}");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                GUILayout.BeginVertical("Box");
                RGUI.BeginBackgroundColor(Color.cyan);
                string[] mapList;
                GUILayout.BeginVertical();
                settings.filterString = RGUI.Field(settings.filterString, "Filter Map List", GUILayout.Width(256));
                GUILayout.EndVertical();
                if (!string.IsNullOrEmpty(settings.filterString))
                {
                    mapList = MapHelper.FilterArray(MapHelper.GetMapNames(), settings.filterString);
                }
                else
                {
                    mapList = MapHelper.GetMapNames();
                }
                settings.selectedMap = RGUI.SelectionPopup(settings.selectedMap, mapList);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Map", RGUIStyle.button, GUILayout.Width(128)))
                {
                    multiMapVote.AddMapToOptions(settings.selectedMap);
                    settings.selectedMap = PopUpLabels.addMapText;
                }
                if (GUILayout.Button("Clear List", RGUIStyle.button, GUILayout.Width(128)))
                {
                    multiMapVote.ClearPopUpOptions();
                }
                GUILayout.EndHorizontal();
                foreach (string option in multiMapVote.popUpOptions)
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.Label(option, GUILayout.Width(256));
                    GUILayout.EndVertical();
                }
                RGUI.EndBackgroundColor();
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical(); // main Box
            */
        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (enabled == value)
                return true;

            enabled = value;

            if (enabled)
            {
                try
                {
                    harmonyInstance = new Harmony(modEntry.Info.Id);
                    harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

                    ScriptManager = new GameObject("XLMultiMapVote");
                    voteController = ScriptManager.AddComponent<VoteController>();
                    mapChangeManager = ScriptManager.AddComponent<MapChangeManager>();
                    networkPlayerManager = ScriptManager.AddComponent<NetworkPlayerManager>();
                    menuButtonManager = ScriptManager.AddComponent<MenuButtonManager>();
                    popupMenuManager = ScriptManager.AddComponent<PopupMenuManager>();
                    uiController = ScriptManager.AddComponent<UIController>();
                    Object.DontDestroyOnLoad(ScriptManager);

                    AssetLoader.LoadBundles();

                    EnableVoting(true);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error during {modEntry} initialization: {ex.Message}");
                    enabled = false; // Rollback enabling if an error occurs
                    return false;
                }
            }
            else
            {
                Unload(modEntry);
            }

            return true;
        }

        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            try
            {
                EnableVoting(false);

                harmonyInstance?.UnpatchAll(harmonyInstance.Id);
                AssetLoader.UnloadAssetBundle();

                if (ScriptManager != null)
                {
                    Object.Destroy(ScriptManager);
                    ScriptManager = null; // Null the reference after destroying to avoid potential issues
                }

                Logger.Log(nameof(Unload));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during {modEntry} unload: {ex.Message}");
                return false;
            }

            return true;
        }
        private static void EnableVoting(bool enabled)
        {
            settings.isVotingEnabled = enabled;
            NetworkPlayerHelper.SetPlayerProperties(enabled);

            if (!enabled && MapHelper.isVoteInProgress)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    voteController.CancelVote(true);
                }
                else
                {
                    voteController.CancelVote(false);
                }
            }
        }
        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;
    }
}
