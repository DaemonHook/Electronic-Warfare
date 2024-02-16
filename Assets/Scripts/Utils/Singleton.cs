using System;
using UnityEngine;

/// <summary>
/// 继承自 MonoBehaviour 的单例
/// </summary>
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> 
// 保证可以在这里使用MonoBehaviourSingleton的方法
{
    /// <summary>
    /// 访问单例的唯一接口
    /// </summary>
    public static T I   // 我嫌 Instance 单词太长了
    {
        get;
        private set;
    }

    public void Awake()
    {
        I = this as T;
    }
}