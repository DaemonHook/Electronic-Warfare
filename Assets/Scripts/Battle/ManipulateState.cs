using System;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// 状态机中的状态
/// </summary>
public class ManipulateState
{
    public static Dictionary<string, ManipulateState> StateDic = new Dictionary<string, ManipulateState>();
    public Action OnEnter { get; set; }
    public Action OnExit { get; set; }
    public Action<UIEvent> OnEvent { get; set; }
    public string Name { get; private set; }

    public StateMachine Machine { get; private set; } 

    public ManipulateState(string name)
    {
        Name = name;
        StateDic.Add(name, this);
    }
    
    public ManipulateState(string name, Action onEnter, Action onExit, Action<object> onEvent, 
        StateMachine machine)
    {
        Name = name;
        StateDic.Add(name, this);
        OnEnter = onEnter;
        OnExit = onExit;
        OnEvent = onEvent;
        Machine = machine;
    }

    public void Event(UIEvent @event)
    {
        OnEvent.Invoke(@event);
    }

    public void Switch(string newState)
    {
        Machine.Switch(newState);
        OnExit?.Invoke();
    }
}

public class StateMachine
{
    public Dictionary<string, ManipulateState> StateDic = new();
    public ManipulateState CurState = null;

    public void AddNewState(ManipulateState state)
    {

    }

    public void Init(string firstStateName)
    {
        
    }

    public void Switch(string newStateName)
    {
        CurState.OnExit?.Invoke();
        CurState = StateDic(newStateName);

    }
}