using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;
using ModIO.UI;

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

        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool state)
        {
            // If the current state is the same as the new state, just return true as nothing needs to change.
            if (enabled == state) return true;

            // Update the 'enabled' flag to the new state.
            enabled = state;

            if (enabled)
            {
                harmonyInstance = new Harmony(modEntry.Info.Id);
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

            return true; // return true to indicate the toggle was processed correctly.
        }
        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            Logger.Log(nameof(Unload));
            return true;
        }

        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;
    }
}
