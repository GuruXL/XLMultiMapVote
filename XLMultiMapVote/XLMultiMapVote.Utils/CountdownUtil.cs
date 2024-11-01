using HarmonyLib;

namespace XLMultiMapVote.Utils
{
    public static class CountdownUtil
    {
        public static float countdownDuration
        {
            get
            {
                return Traverse.Create(CountdownUI.Instance).Field("durationLeft").GetValue<float>();
            }
        }
    }
}