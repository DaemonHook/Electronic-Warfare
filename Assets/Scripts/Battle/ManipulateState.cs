using System;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// 状态机
/// </summary>
public class ManipulateState
{
    public static Dictionary<string, ManipulateState> StateDic = new Dictionary<string, ManipulateState>();
    public Action OnEnter { get; set; }
    public Action OnExit { get; set; }
    public Action<object> OnEvent { get; set; }
    public string Name { get; private set; }

    public ManipulateState(string name)
    {
        Name = name;
        StateDic.Add(name, this);
    }
    
    public ManipulateState(string name, Action onEnter, Action onExit, Action<object> onEvent)
    {
        Name = name;
        StateDic.Add(name, this);
        OnEnter = onEnter;
        OnExit = onExit;
        OnEvent = onEvent;
    }

    public void Event(object param)
    {
        OnEvent.Invoke(param);
    }

    public ManipulateState SwitchTo(string nextStateName)
    {
        OnExit?.Invoke();
        ManipulateState nextState = StateDic[nextStateName];
        nextState.OnEnter?.Invoke();
        return nextState;
    }

    public ManipulateState SwitchTo(ManipulateState newState)
    {
        OnExit?.Invoke();
        newState.OnEnter?.Invoke();
        return newState;
    }
}