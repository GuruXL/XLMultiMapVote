using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;
using ModIO.UI;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;
using XLMultiMapVote.UI;

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
        public static XLMultiMapVote multiMapVote;
        public static UIController uiController;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = new System.Action<UnityModManager.ModEntry>(OnSaveGUI);
            modEntry.OnToggle = new System.Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
            modEntry.OnUnload = new System.Func<UnityModManager.ModEntry, bool>(Unload);
            Main.modEntry = modEntry;
            Logger.Log(nameof(Load));

            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!multiMapVote.IsInRoom())
                return;

            GUILayout.BeginVertical("Box"); // Main Box

            GUILayout.BeginHorizontal();
            RGUI.BeginBackgroundColor(Color.white);
            settings.allowPopUps = GUILayout.Toggle(settings.allowPopUps, "Allow PopUps", GUILayout.Width(128));
            RGUI.EndBackgroundColor();
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            if (multiMapVote.IsHost())
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
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            bool flag;
            if (enabled == value)
            {
                flag = true;
            }
            else
            {
                enabled = value;
                if (enabled)
                {
                    harmonyInstance = new Harmony((modEntry.Info).Id);
                    harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                    ScriptManager = new GameObject("XLMultiMapVote");
                    multiMapVote = ScriptManager.AddComponent<XLMultiMapVote>();
                    uiController = ScriptManager.AddComponent<UIController>();
                    Object.DontDestroyOnLoad(ScriptManager);

                    AssetLoader.LoadBundles();
                }
                else
                {
                    harmonyInstance.UnpatchAll(harmonyInstance.Id);
                    AssetLoader.UnloadAssetBundle();
                    Object.Destroy(ScriptManager);
                }
                flag = true;
            }
            return flag;
        }
        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmonyInstance.UnpatchAll(harmonyInstance.Id);
            AssetLoader.UnloadAssetBundle();
            Object.Destroy(ScriptManager);
            Logger.Log(nameof(Unload));
            return true;
        }

        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;
    }
}
