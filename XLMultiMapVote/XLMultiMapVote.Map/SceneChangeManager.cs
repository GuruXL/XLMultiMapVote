using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLMultiMapVote.Utils;

namespace XLMultiMapVote.Map
{
    /*
    public class SceneChangeManager : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
            {
                return;
            }

            if (PhotonNetwork.IsMasterClient && scene.name != "LoadingScene")
            {
                MapHelper.SetCurrentLevel();

                if (MapHelper.HasMapChanged())
                {
                    Main.multiMapVote.StopMapChangeRoutines();
                    PlayerController.Instance.respawn.ForceRespawn();
                }
            }

            //Main.Logger.Log("Scene Loaded: " + scene.name);
        }

        private void OnSceneUnloaded(Scene scene)
        {

            //Main.Logger.Log("Scene Unloaded: " + scene.name);
        }
    }
    */
}