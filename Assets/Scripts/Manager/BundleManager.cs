using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BundleManager : BaseManager<BundleManager>
{
#if UNITY_EDITOR
    public bool editorMode = true;
#else
    public bool editorMode = false;
#endif

    private string[] allAssetBundles;

    public string CurrentBundle { get; set; }

    /// <summary>
    /// 已经被加载的 bundle
    /// </summary>
    private Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 所有包的元信息
    /// </summary>
    private AssetBundleManifest manifest;

    /// <summary>
    /// 从根 bundle 获取所有的已安装 bundle 元信息
    /// </summary>
    private void LoadBundleInfos()
    {
        // 读取 manifest 所在的 bundle
        AssetBundle manifestAb = AssetBundle.LoadFromFile(Path.Combine(ResourceConfig.BundlePath, "bundles"));

        manifest = manifestAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        allAssetBundles = manifest.GetAllAssetBundles();

        if (!allAssetBundles.Contains("default"))
        {
            Debug.LogError("没有默认包，无法加载游戏");
        }

        // 先加载 default 包
        // LoadBundle("default");
    }

    public void LoadBundle(string bundleName, Action onComplete = null)
    {
        if (!allAssetBundles.Contains(bundleName))
        {
            Debug.LogError("没有找到 " + bundleName + " bundle");
        }

        var dependencies = manifest.GetAllDependencies(bundleName);
        foreach (var dependency in dependencies)
        {
            LoadBundle(bundleName);
        }

        StartCoroutine(LoadBundleCoroutine(bundleName, onComplete));
        // loadedBundles.Add(bundleName, AssetBundle.LoadFromFileAsync(Path.Combine(ResourceConfig.BundlePath, bundleName)));
    }

    IEnumerator LoadBundleCoroutine(string bundleName, Action onComplete)
    {
        var req = AssetBundle.LoadFromFileAsync(Path.Combine(ResourceConfig.BundlePath, bundleName));
        yield return req;
        loadedBundles.Add(bundleName, req.assetBundle);
        onComplete?.Invoke();
        Debug.Log("加载了 " + bundleName + " bundle");
    }

    public override void InitManager()
    {
        base.InitManager();

        LoadBundleInfos();
    }

    public AssetBundle GetBundle(string bundleName)
    {
        return loadedBundles[bundleName];
    }
}