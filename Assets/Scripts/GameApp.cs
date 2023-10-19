using System.Collections;
using System.Collections.Generic;
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
        var mapTAList = Resources.LoadAll<TextAsset>($"{ModuleName}/Maps");
        return new List<string>(from mapTA in mapTAList select mapTA.name);
    }

    public static void EnterMap(string mapName)
    {
        MapName = mapName;
        var ao = SceneManager.LoadSceneAsync("Scenes/Battle");
    }
}