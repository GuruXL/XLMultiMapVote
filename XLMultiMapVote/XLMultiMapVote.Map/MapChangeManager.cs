using ModIO.UI;
using Photon.Pun;
using SkaterXL.Data;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLMultiMapVote;
using XLMultiMapVote.Data;

namespace XLMultiMapVote.Map
{
    public class MapChangeManager : MonoBehaviour
    {
        private void Awake()
        {
            LevelManager.Instance.OnLevelChanged += HandleLevelChanged;
        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnLevelChanged -= HandleLevelChanged;
        }

        private void HandleLevelChanged(LevelInfo levelInfo)
        {
            if (!PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
            {
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                MapHelper.SetCurrentLevel(levelInfo, false);

                if (MapHelper.HasMapChanged() && MapHelper.hasMapChangedByVote)
                {
                    Main.multiMapVote.StopMapChangeRoutines();

                    PlayerController.Instance.respawn.ForceRespawn();

                    MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.mapChangedMessage + levelInfo.name, 1.5f);
                    Main.Logger.Log(Labels.mapChangedMessage + levelInfo.name);

                    MapHelper.Set_hasMapChangedByVote(false);
                }
            }

            //Main.Logger.Log("HandleLevelChanged Called");
        }
    }
}