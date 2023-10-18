using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class GameApp
{
    public static string PackageName { get; set; } = "SamplePackage";

    public static List<string> GetMapList()
    {
        var maps = Resources.LoadAll<TextAsset>($"{PackageName}/Maps");
        List<string> mapNames = new List<string>(
            from map in maps select Path.GetFileNameWithoutExtension(map.name));
        return mapNames;
    }
}