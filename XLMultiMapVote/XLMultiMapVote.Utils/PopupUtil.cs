using HarmonyLib;

namespace XLMultiMapVote.Utils
{
    public static class PopupUtil
    {
        public static void TimeoutPopup()
        {
            AccessTools.Method(typeof(MultiplayerGameModePopup), "TimeOut").Invoke(MultiplayerManager.Instance.gameModePopup, null);
        }
    }
}