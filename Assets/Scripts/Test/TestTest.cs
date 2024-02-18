using System;
using System.Collections;
using System.IO;
using System.Resources;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

namespace Test
{
    public class MyClass
    {
        public void PrintClassName()
        {
            Console.WriteLine("Class name: " + typeof(MyClass).Name);
        }
    }
    [TestFixture]
    public class TestTest
    {
       
        [Test]
        public void LoadFromFile()
        {
            
            // var asset = AssetBundle.LoadFromFile(ResourceConfig.BundlePath);
            // var manifest = asset.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            // var allAssetBundles = manifest.GetAllAssetBundles();
            // foreach (var allAssetBundle in allAssetBundles)
            // {
            //     Debug.Log(allAssetBundle);
            // }
        }
    }
}