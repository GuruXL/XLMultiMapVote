using HarmonyLib;

namespace XLMultiMapVote.Utils
{
    public static class PopupUtil
    {
        public static float popUpTime = 30.0f;

        public static void TimeoutPopup()
        {
            AccessTools.Method(typeof(MultiplayerGameModePopup), "TimeOut").Invoke(MultiplayerManager.Instance.gameModePopup, null);
        }
        public static bool isPaused()
        {
            return Traverse.Create(MultiplayerManager.Instance.gameModePopup).Field("pause").GetValue<bool>();
        }
    }
}