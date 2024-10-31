﻿using System.Reflection;
using System.IO;

namespace XLMultiMapVote.Utils
{
    public static class ControlsHelper
    {
        public static void EnableActiveControls(bool enabled)
        {
            if (InputController.Instance == null)
            {
                return;
            }

            if (!InputController.Instance.controlsActive) // OnDisable is not being called on gameModePopup, This is Temp fix for frozen player controls
            {
                InputController.Instance.controlsActive = enabled;
            }
        }
    }
}