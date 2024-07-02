using System;
using UnityEngine;

/// <summary>
/// 继承自 MonoBehaviour 的单例
/// </summary>
public class MBSingleton<T> : MonoBehaviour where T : MonoBehaviour
// 保证可以在这里使用MBSingleton的方法
{
    /// <summary>
    /// 访问单例的唯一接口
    /// </summary>
    public static T I   // 我嫌 Instance 单词太长了
    {
        get;
        private set;
    }

    public virtual void Awake()
    {
        I = this as T;
    }
}