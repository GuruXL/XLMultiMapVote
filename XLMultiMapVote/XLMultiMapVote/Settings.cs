using Photon.Pun;
using System;
using UnityModManagerNet;
using XLMultiMapVote.Network;

namespace XLMultiMapVote
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public bool isVotingEnabled = true;
       
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
