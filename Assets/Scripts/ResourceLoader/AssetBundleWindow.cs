using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class AssetBundleWindow : EditorWindow
{
    private string _buildPath = Application.streamingAssetsPath + "/AssetBundles";
    private static string[] _bundleNames = null;
    private static bool[] _buildTargets = null;
    private BuildAssetBundleOptions _buildOption = BuildAssetBundleOptions.UncompressedAssetBundle;
    private BuildTarget _buildTarget = BuildTarget.StandaloneWindows;

    [MenuItem("AssetBundle/Asset Bundle Window")]
    static void Init()
    {
        var bundles = AssetDatabase.GetAllAssetBundleNames();
        if (_bundleNames == null || !_bundleNames.SequenceEqual(bundles))
        {
            _bundleNames = bundles;
            _buildTargets = new bool[_bundleNames.Length];
        }

        var window = (AssetBundleWindow)EditorWindow.GetWindow(typeof(AssetBundleWindow), true, "에셋 번들 선택 빌드");
        window.maxSize = new Vector2(500, 800);
        window.maximized = true;
        window.Show();
    }

    private void OnGUI()
    {
        _buildPath = EditorGUILayout.TextField("Build Path", _buildPath);
        if (GUILayout.Button("Browse"))
        {
            _buildPath = EditorUtility.OpenFolderPanel("Select build path", _buildPath, "");
        }
         
        for (int i = 0; i<_buildTargets.Length; i++)
        {
            _buildTargets[i] = EditorGUILayout.Toggle(_bundleNames[i], _buildTargets[i]);
        }

        _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", _buildTarget);

        _buildOption = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Build Option", _buildOption);

        if (GUILayout.Button("BUILD"))
        {
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

            for (int i = 0; i < _buildTargets.Length; i++)
            {
                if (!_buildTargets[i]) continue;

                var build = new AssetBundleBuild();
                build.assetBundleName = _bundleNames[i];
                build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(_bundleNames[i]);

                builds.Add(build);
            }
            
            if(builds.Count > 0)
            {
                BuildPipeline.BuildAssetBundles(_buildPath, builds.ToArray(), _buildOption, _buildTarget);

                Debug.Log("[AssetBundleWindow] Asset Bundle Build Success!");
            }
        }
    }
}
#endif