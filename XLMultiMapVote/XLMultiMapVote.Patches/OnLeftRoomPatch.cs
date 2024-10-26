using HarmonyLib;
using SkaterXL.Multiplayer;
using UnityEngine;
using Photon.Pun;
using ModIO.UI;

namespace XLMultiMapVote.Patches
{
    
    [HarmonyPatch(typeof(MultiplayerManager))]
    [HarmonyPatch("OnLeftRoom")]
    public static class OnLeftRoomPatch
    {
        public static void Postfix()
        {
            if (!Main.multiMapVote.isMapchanging)
                return;

            if (PhotonNetwork.IsMasterClient)
            {
                Main.multiMapVote.CancelVote(true);

                //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"Cancelled Over Network", 2.0f);
            }
            else
            {
                Main.multiMapVote.CancelVote(false);

                MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"Cancelled Locally", 2.0f);
            }

            //MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, $"OnLeftRoom Patch Has Run", 2.0f);
        }
    }
    
}