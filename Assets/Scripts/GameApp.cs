using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏的总控制
/// </summary>
public class GameApp: MonoBehaviourSingleton<GameApp>
{
    // 当前模组名称
    // public string ModuleName { get; set; } = "SamplePackage";
    //
    // public string MapName { get; set; } = null;

    // public List<string> GetMapList()
    // {
    //     var mapTaList = Resources.LoadAll<TextAsset>($"{ModuleName}/Maps") ?? throw new ArgumentNullException("Resources.LoadAll<TextAsset>($\"{ModuleName}/Maps\")");
    //     return new List<string>(
    //         from mapTa in mapTaList select Path.GetFileNameWithoutExtension(mapTa.name));
    // }
    //
    // public void EnterMap(string mapName)
    // {
    //     MapName = mapName;
    //     var ao = SceneManager.LoadSceneAsync("Scenes/Battle");
    // }
    
    
    // public static void StartBattle()
    // {
    //     
    // }
    
    /// <summary>
    /// 初始化最基础的几个管理器
    /// </summary>
    private void InitBaseManagers()
    {
        Debug.Log("InitBaseManagers");
        BundleManager.I.InitManager();
        EventManager.I.InitManager();
        
        BundleManager.I.LoadBundle("default", () =>
        {
            Debug.Log("加载默认 bundle");
        });
        
        
    }

    private void Start()
    {
        // 保证 GameApp 的全局存在
        DontDestroyOnLoad(this);
        InitBaseManagers();
    }

    private void StartGame()
    {
        
    }
}