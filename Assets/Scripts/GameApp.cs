using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameApp
{
    // 当前模组名称
    public static string ModuleName { get; set; } = "SamplePackage";

    public static string MapName { get; set; } = null;

    public static List<string> GetMapList()
    {
        var mapTaList = Resources.LoadAll<TextAsset>($"{ModuleName}/Maps") ?? throw new ArgumentNullException("Resources.LoadAll<TextAsset>($\"{ModuleName}/Maps\")");
        return new List<string>(
            from mapTa in mapTaList select Path.GetFileNameWithoutExtension(mapTa.name));
    }

    public static void EnterMap(string mapName)
    {
        MapName = mapName;
        var ao = SceneManager.LoadSceneAsync("Scenes/Battle");
    }
}