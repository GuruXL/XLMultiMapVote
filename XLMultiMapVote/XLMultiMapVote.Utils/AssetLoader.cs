using UnityEngine;
using GameManagement;
using System.Collections;
using System;
using ModIO.UI;

namespace XLMultiMapVote.Utils
{
    public static class AssetLoader
    {
        private static AssetBundle assetBundle;

        public static GameObject MapVoteUIPrefab { get; private set; }

        public static bool assetsLoaded { get; private set; } = false;

        public static void LoadBundles()
        {
            // Check if a type from the Unity assembly has been loaded
            Type unityObjectType = Type.GetType("UnityEngine.Object, UnityEngine");

            if (unityObjectType != null)
            {
                GameStateMachine.Instance.StartCoroutine(LoadAssetBundle());
            }
        }   
        private static IEnumerator LoadAssetBundle()
        {
            byte[] assetBundleData = ResourceExtractor.ExtractResources("XLMultiMapVote.Resources.mapvoteui");
            if (assetBundleData == null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"Failed to EXTRACT XLMultiMapVote Asset Bundle", 2.5f);
                assetsLoaded = false;
                yield break;
            }
            AssetBundleCreateRequest abCreateRequest = AssetBundle.LoadFromMemoryAsync(assetBundleData);
            yield return abCreateRequest;

            assetBundle = abCreateRequest.assetBundle;
            if (assetBundle == null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"Failed to LOAD XLMultiMapVote Asset Bundle", 2.5f);
                assetsLoaded = false;
                yield break;
            }
            yield return GameStateMachine.Instance.StartCoroutine(LoadAssetFromBundle());
        }

        private static IEnumerator LoadAssetFromBundle()
        {
            MapVoteUIPrefab = assetBundle.LoadAsset<GameObject>("MapVoteUI");
            assetsLoaded = true;
            yield return null;
        }       
        public static void UnloadAssetBundle()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
                assetsLoaded = false;
            }
        }
        private static void OnDestroy()
        {
            UnloadAssetBundle();
        }
    }
}
