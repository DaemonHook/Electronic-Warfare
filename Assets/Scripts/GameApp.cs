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
public class GameApp: MBSingleton<GameApp>
{
    /// <summary>
    /// 初始化最基础的几个管理器
    /// </summary>
    private void InitBaseManagers()
    {
        Debug.Log("InitBaseManagers");
        BundleManager.I.InitManager();
        EventManager.I.InitManager();
        ViewManager.I.InitManager();
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