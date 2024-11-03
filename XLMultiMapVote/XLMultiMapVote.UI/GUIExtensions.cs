using Photon.Pun;
using RapidGUI;
using SkaterXL.Multiplayer;
using UnityEngine;

namespace XLMultiMapVote.UI
{
    public static class GUIExtensions
    {
        public static Color ColorSwitch(bool enabled, Color color1, Color color2)
        {
            switch (enabled)
            {
                case true:
                    return color1;
                case false:
                    return color2;
            }
            return Color.white;
        }

        public static string LabelSwitch(string label1, Color color1, string label2, Color color2, bool enabled)
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(enabled ? color1 : color2);
            string label = enabled ? label1 : label2;

            return $"<b><color=#{colorHex}> {label} </color></b>";
        }
    }
}
