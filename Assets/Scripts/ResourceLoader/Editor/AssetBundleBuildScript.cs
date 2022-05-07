using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;


namespace CellBig
{
    public static class AssetBundleBuildScript
    {
        public static void BuildAssetBundles()
        {
            string fullPath = String.Format("{0}/StreamingAssets/AssetBundles/", Application.dataPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            BuildPipeline.BuildAssetBundles(fullPath, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.DisableWriteTypeTree, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
