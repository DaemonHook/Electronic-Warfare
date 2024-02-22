using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 所有的 Manager 都应该挂在 GameApp 上
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseManager<T> : MonoBehaviourSingleton<T> where T : BaseManager<T>
{
    public string ManagerName { get; protected set; }

    public override void Awake()
    {
        base.Awake();
        ManagerName = typeof(T).Name;
    }

    public virtual void InitManager()
    {
        Debug.Log($"Manager {ManagerName} is initialized.");
    }
}