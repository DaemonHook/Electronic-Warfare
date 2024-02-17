using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Tools/BuildAssetBundles")]
    static void BuildAllAssetBundles()
    {
        string dir = ResourceConfig.BundlePath;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
#if UNITY_IOS
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.iOS);
#elif UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.Android);
#elif UNITY_STANDALONE_OSX
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
#elif UNITY_STANDALONE_WIN
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#else
        Debug.LogError("Invalid Build target");
#endif
    }

    // [MenuItem("Tools/BuildAssetBundlesForIOS")]
    // static void BuildAllAssetBundlesForIOS()
    // {
    //     string dir = ;
    //     if (!Directory.Exists(dir))
    //     {
    //         Directory.CreateDirectory(dir);
    //     }
    //     BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.iOS);
    // }
}