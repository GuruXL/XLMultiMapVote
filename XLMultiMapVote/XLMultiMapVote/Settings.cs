using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;

namespace XLMultiMapVote
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {

        public void OnChange()
        {
            throw new NotImplementedException();
        }
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
