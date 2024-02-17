using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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