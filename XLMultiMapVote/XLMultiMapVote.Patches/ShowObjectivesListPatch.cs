using HarmonyLib;
using Photon.Pun;

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