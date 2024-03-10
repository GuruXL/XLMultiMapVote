using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;
using ModIO.UI;
using XLMultiMapVote.Data;

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
            if (!MultiplayerManager.Instance.InRoom)
                return;

            GUILayout.BeginVertical("Box"); // Main Box

            GUILayout.BeginHorizontal();
            RGUI.BeginBackgroundColor(Color.white);
            settings.allowPopUps = GUILayout.Toggle(settings.allowPopUps, "Allow PopUps", GUILayout.Width(128));
            RGUI.EndBackgroundColor();
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            GUILayout.Label("Options");
            GUILayout.BeginHorizontal();
            RGUI.BeginBackgroundColor(Color.cyan);
            if (GUILayout.Button("Show PopUp", RGUIStyle.button, GUILayout.Width(128)))
            {
                multiMapVote.ShowPlayerPopUp(PopUpMessage.message, true, 10f);
            }
            RGUI.EndBackgroundColor();
            GUILayout.EndHorizontal();

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

                    Object.DontDestroyOnLoad(ScriptManager);
                }
                else
                {
                    harmonyInstance.UnpatchAll(harmonyInstance.Id);
                    Object.Destroy(ScriptManager);
                }
                flag = true;
            }
            return flag;
        }
        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            Logger.Log(nameof(Unload));
            return true;
        }

        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;
    }
}
