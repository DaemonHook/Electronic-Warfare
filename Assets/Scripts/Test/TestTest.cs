using System;
using System.IO;
using System.Resources;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

namespace Test
{
    [TestFixture]
    public class TestTest
    {
        [Test]
        public void LoadFromFile()
        {
            var assetBundle = AssetBundle.LoadFromFile(ResourceConfig.BundlePath + "/default");
            var t = assetBundle.LoadAsset<TextAsset>("meta").text;
            Debug.Log(t);
            PropertyLoader propertyLoader = new PropertyLoader(new StringReader(t));
            Debug.Log(propertyLoader.GetProperty("root"));
        }
    }
}