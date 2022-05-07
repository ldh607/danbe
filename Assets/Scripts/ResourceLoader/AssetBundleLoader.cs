#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CellBig.Common;


namespace CellBig
{
    public sealed class AssetBundleLoader : MonoSingleton<AssetBundleLoader>
    {
        private Dictionary<string, AssetBundle> _loadBundles = new Dictionary<string, AssetBundle>();
        private Dictionary<string, AssetBundleCreateRequest> _loadBundleProcess = new Dictionary<string, AssetBundleCreateRequest>();
        private string _bundlePath = $"{Application.streamingAssetsPath}/AssetBundles/";
        private string _editorPath = "Assets/AssetBundle/";

        static bool iseditorLoad = true;
        public static bool isEditorLoad
        {
            get
            {
#if UNITY_EDITOR
                return iseditorLoad;
#else
                return false;
#endif
            }
            set
            {
                iseditorLoad = value;
            }
        }

        private IEnumerator _loadAssetProcess = null;

        protected override void Init()
        {
            base.Init();

#if !UNITY_EDITOR
            isEditorLoad = false;
#endif
        }

        public T Load<T>(string bundleName, string assetName, string assetType = ".prefab") where T : Object
        {

#if UNITY_EDITOR
            if (isEditorLoad)
            {
                return AssetDatabase.LoadAssetAtPath<T>($"{_editorPath}{bundleName}/{assetName}{assetType}");
            }
#endif

            bundleName = bundleName.ToLower();
            assetName = Path.GetFileName(assetName);

            if (!_loadBundles.ContainsKey(bundleName))
            {
                _loadBundles.Add(bundleName, AssetBundle.LoadFromFile($"{_bundlePath}{bundleName}"));
            }

            try
            {
                if (_loadBundles[bundleName])
                {
                    return _loadBundles[bundleName].LoadAsset<T>(assetName);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"on load asset from bundle fail : {bundleName} - {assetName}");
                return null;
            }

            return null;
        }

        private bool isOnLoadAsset() { return _loadAssetProcess != null; }

        public IEnumerator LoadAsync<T>(string bundleName, string assetName, System.Action<T> onLoad, string assetType = ".prefab") where T : Object
        {
            yield return new WaitWhile(isOnLoadAsset);

#if UNITY_EDITOR
            if (isEditorLoad)
            {
                string path = "";
                if (bundleName == "")
                {
                    path = $"{_editorPath}{assetName}{assetType}";
                }
                else
                {
                    path = $"{_editorPath}{bundleName}/{assetName}{assetType}";
                }

                var obj = AssetDatabase.LoadAssetAtPath<T>(path);
                onLoad(obj);

                yield break;
            }
#endif
            _loadAssetProcess = LoadAsyncInternal(bundleName, assetName, onLoad);
            yield return StartCoroutine(_loadAssetProcess);
        }

        private IEnumerator LoadAsyncInternal<T>(string bundleName, string assetName, System.Action<T> onLoad) where T : Object
        {
            bundleName = bundleName.ToLower();
            assetName = Path.GetFileName(assetName);

            if (!_loadBundles.ContainsKey(bundleName))
            {
                if (!_loadBundleProcess.ContainsKey(bundleName))
                {
                    var requestBundle = AssetBundle.LoadFromFileAsync($"{_bundlePath}{bundleName}");
                    _loadBundleProcess.Add(bundleName, requestBundle);
                    yield return requestBundle;

                    try
                    {
                        _loadBundles.Add(bundleName, requestBundle.assetBundle);
                        _loadBundleProcess.Remove(bundleName);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("on LoadAsyncInternal fail : " + e);
                    }
                }
                else
                {
                    yield return new WaitWhile(() => _loadBundleProcess.ContainsKey(bundleName));
                }
            }
            var requestAsset = _loadBundles[bundleName].LoadAssetAsync<T>(assetName);
            yield return requestAsset;

            try
            {
                onLoad(requestAsset.asset as T);
            }
            catch (System.Exception e)
            {
                onLoad(null);
            }

            _loadAssetProcess = null;
        }

        public void Unload(string assetBundleName)
        {
        }
    }
}
