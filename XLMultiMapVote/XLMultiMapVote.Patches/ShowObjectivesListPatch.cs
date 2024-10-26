using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ModIO.UI;
using UnityEngine.Rendering.HighDefinition;

namespace XLMultiMapVote.Patches
{ 
    [HarmonyPatch(typeof(NetworkPlayerController))]
    [HarmonyPatch("ShowObjectivesList")]
    class ShowObjectivesListPatch
    {
        public static bool Prefix()
        {
            if (!PhotonNetwork.IsMasterClient && Main.settings.disableVotingForSelf) // no cheat detection for objective list. added so user can disable all UI.
            {
                return false; // Skip Original Method
            }
            else
            {
                return true; // Continue
            }
        }
    }
    
}