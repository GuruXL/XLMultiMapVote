using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLMultiMapVote;

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
                MapHelper.SetCurrentLevel(levelInfo);

                if (MapHelper.HasMapChanged())
                {
                    Main.multiMapVote.StopMapChangeRoutines();
                    PlayerController.Instance.respawn.ForceRespawn();
                }
            }
        }
    }
}