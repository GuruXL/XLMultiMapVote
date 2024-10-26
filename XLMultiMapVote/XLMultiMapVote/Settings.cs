using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
using XLMultiMapVote.Data;
using RapidGUI;

namespace XLMultiMapVote
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public bool disableVotingForSelf = false;
        //public bool allowPopUps = true;

        public float popUpTime = 30.0f;

        public string selectedMap = Labels.addMapText;

        public string filterString = "";

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
