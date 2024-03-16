using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameManagement;
using ModIO.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using XLMultiMapVote.Data;
using XLMultiMapVote.Utils;

namespace XLMultiMapVote.UI
{
    public class UIController : MonoBehaviour
    {
        public GameObject mapVoteUIobj;

        private void Awake()
        {
            StartCoroutine(CreateCustomUI());
        }

        private IEnumerator CreateCustomUI()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);

            mapVoteUIobj = Instantiate(AssetLoader.MapVoteUIPrefab);
            mapVoteUIobj.transform.SetParent(Main.ScriptManager.transform);
            mapVoteUIobj.SetActive(false);
        }
    }
}
