using System;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// 状态机中的状态
/// </summary>
public class ManipulateState
{
    /*
     * 状态机事件函数
     * 其参数为
     */
    public Action<int> OnEnter { get; set; }
    public Action<int> OnExit { get; set; }
    public Action<UIEvent, int> OnEvent { get; set; }
    public string Name { get; private set; }

    public StateMachine Machine { get; private set; } 

    public ManipulateState(string name)
    {
        Name = name;
    }
}

public class StateMachine
{
    public Dictionary<string, ManipulateState> StateDic = new();
    public ManipulateState CurState = null;

    public void AddNewState(ManipulateState state)
    {
        StateDic.Add(state.Name, state);
    }

    public void Init(string firstStateName)
    {
        
    }

    public void Switch(string newStateName)
    {
        CurState.OnExit?.Invoke();
        CurState = StateDic[newStateName];

    }
}