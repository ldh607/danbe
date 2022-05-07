using UnityEditor;
using UnityEngine;


namespace CellBig
{
    public static class AssetbundlesMenuItems
    {
        const string simulateAssetBundlesMenu = "CellBig/AssetBundles/Simulate AssetBundles";


        [MenuItem(simulateAssetBundlesMenu)]
        public static void ToggleSimulateAssetBundle()
        {
            AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
        }

        [MenuItem(simulateAssetBundlesMenu, true)]
        public static bool ToggleSimulateAssetBundleValidate()
        {
            Menu.SetChecked(simulateAssetBundlesMenu, AssetBundleManager.SimulateAssetBundleInEditor);
            return true;
        }

        [MenuItem("CellBig/AssetBundles/Build AssetBundles", false, 100)]
        static public void BuildAssetBundles()
        {
            AssetBundleBuildScript.BuildAssetBundles();
        }
    }
}
