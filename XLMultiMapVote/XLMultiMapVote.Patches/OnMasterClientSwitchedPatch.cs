using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using ModIO.UI;
using XLMultiMapVote.Map;

namespace XLMultiMapVote.Patches
{
    /*
    [HarmonyPatch(typeof(MultiplayerManager))]
    [HarmonyPatch("OnMasterClientSwitched")]
    class OnMasterClientSwitchedPatch
    {
        public static void Postfix(Player newMasterClient)
        {
            if (!MapHelper.isVoteInProgress && !PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }

            if (newMasterClient.IsLocal)
            {
                Main.multiMapVote.CancelVote(true);
                //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"newMasterClient Cancelled Over Network", 2.0f);
            }
            else
            {
                Main.multiMapVote.CancelVote(false);
                //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"newMasterClient Cancelled Locally", 2.0f);
            }
        }
    }
    */
}