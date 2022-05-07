#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.IO;


namespace CellBig
{
	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class LoadedAssetBundle
	{
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;

		public LoadedAssetBundle(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
			m_ReferencedCount = 1;
		}
	}

	// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
	public class AssetBundleManager : MonoBehaviour
	{
		static bool initializeComplete = false;
		public static GameObject instance { get; private set; }

#if UNITY_EDITOR
		static int m_SimulateAssetBundleInEditor = -1;
		const string kSimulateAssetBundles = "SimulateAssetBundles";
#endif

		static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();

#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public static bool SimulateAssetBundleInEditor
		{
			get
			{
				if (m_SimulateAssetBundleInEditor == -1)
					m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

				return m_SimulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor)
				{
					m_SimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}
#endif

		// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
		public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName)
		{
			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			return bundle;
		}

		// Load AssetBundleManifest.
		public static void Initialize(string manifestAssetBundleName)
		{
			if (initializeComplete)
				return;

			instance = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
			instance.hideFlags = HideFlags.HideInHierarchy;
			DontDestroyOnLoad(instance);

			initializeComplete = true;
		}

		// Unload assetbundle and its dependencies.
		public static void UnloadAssetBundle(string assetBundleName)
		{
#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return;
#endif

			//Debug.LogFormat("--> {0} assetbundle(s) in memory before unloading \"{1}\"", m_LoadedAssetBundles.Count, assetBundleName);
			UnloadAssetBundleInternal(assetBundleName);
		}

		protected static int UnloadAssetBundleInternal(string assetBundleName)
		{
			LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName);
			if (bundle == null)
				return 0;

			int refCount = --bundle.m_ReferencedCount;
			if (refCount == 0)
			{
				bundle.m_AssetBundle.Unload(false);
				m_LoadedAssetBundles.Remove(assetBundleName);
				//Debug.LogFormat("AssetBundle {0} has been unloaded successfully", assetBundleName);
			}

			return refCount;
		}

		// Load asset from the given assetBundle.
		public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
		{
			AssetBundleLoadAssetOperation operation = null;

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
					return null;
				}

				// @TODO: Now we only get the main object from the first asset. Should consider type also.
				//Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);

				Object target = null;
				for (int i = 0; i < assetPaths.Length; ++i)
				{
					target = AssetDatabase.LoadAssetAtPath(assetPaths[i], type);
					if (target != null)
						break;
				}

				if (target == null)
				{
					Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
					return null;
				}

				operation = new AssetBundleLoadAssetOperationSimulation(target);
			}
			else
#endif
			{
                Object target = LoadAssetBundle(assetBundleName, assetName);
                operation = new AssetBundleLoadAssetOperationSimulation(target);
			}

			return operation;
		}

		// Load asset from the given assetBundle.
		public static AssetBundleLoadAssetOperation LoadAsset(string assetBundleName, string assetName, System.Type type)
		{
			AssetBundleLoadAssetOperation operation = null;

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
					return null;
				}

				// @TODO: Now we only get the main object from the first asset. Should consider type also.
				//Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);

				Object target = null;
				for (int i = 0; i < assetPaths.Length; ++i)
				{
					target = AssetDatabase.LoadAssetAtPath(assetPaths[i], type);
					if (target != null)
						break;
				}

				if (target == null)
				{
					Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
					return null;
				}

				operation = new AssetBundleLoadAssetOperationSimulation(target);
			}
			else
#endif
			{
                Object target = LoadAssetBundle(assetBundleName, assetName);
                operation = new AssetBundleLoadAssetOperationSimulation(target);
            }

			return operation;
		}

        protected static Object LoadAssetBundle(string assetBundleName, string assetName)
        {
            string path = string.Format("{0}/AssetBundles/{1}", Application.streamingAssetsPath, assetBundleName);
            var assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
                return null;

            m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetBundle));
            var obj = assetBundle.LoadAsset<Object>(assetName);

            return obj;
        }

#if UNITY_EDITOR
		public static void LogNotUnloadingAssetBundles()
		{
			foreach (var keyValue in m_LoadedAssetBundles)
			{
				var assetBundle = keyValue.Value;
				if (assetBundle != null && assetBundle.m_AssetBundle != null)
				{
					Debug.LogWarningFormat("Not unloading assetBundle : \"{0}\", RefCount : {1}", keyValue.Key, assetBundle.m_ReferencedCount);
				}
			}
		}
#endif
		public static void UnloadAll()
		{
			foreach (string key in m_LoadedAssetBundles.Keys)
				UnloadAssetBundle(key);
		}
	}
}
