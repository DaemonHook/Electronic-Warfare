/*
 * file: EventManager
 * feature: 全局事件管理器
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件管理器
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    
    private Dictionary<string, Action> registeredEvents;    //已注册的事件

    private void Awake()
    {
        Instance = this;
        registeredEvents = new Dictionary<string, Action>();
    }

    public void RegisterEvent(string eventName, Action action)
    {
        if (registeredEvents.ContainsKey(eventName))
        {
            registeredEvents[eventName] += action;
        }
        else
        {
            registeredEvents.Add(eventName, action);
        }
    }

    public void UnregisterEvent(string eventName, Action action)
    {
        if (!registeredEvents.Remove(eventName))
        {
            Debug.LogError($"event {eventName} does not exist.");
        }
    }

    public void TriggerEvent(string eventName)
    {
        registeredEvents[eventName].Invoke();
    }
}