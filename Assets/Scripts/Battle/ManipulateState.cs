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
     * 其参数为状态机（每个玩家一个）
     * 每个状态机都是全局的，所以需要参数指明当前的玩家
     */
    public Action<StateMachine> OnEnter { get; set; }
    public Action<StateMachine> OnExit { get; set; }
    public Action<UIEvent, StateMachine> OnEvent { get; set; }
    public string Name { get; private set; }

    //
    public StateMachine bindedMachine;

    public StateMachine Machine { get; private set; } 
    
    public ManipulateState(string name)
    {
        Name = name;
    }
}

public class StateMachine
{
    public int Team { get; private set; }
    public Dictionary<string, ManipulateState> StateDic = new();
    public ManipulateState CurState = null;

    public StateMachine(int team)
    {
        Team = team;
    }
    
    public void AddNewState(ManipulateState state)
    {
        StateDic.Add(state.Name, state);
    }

    public void Init(string firstStateName)
    {
        CurState = StateDic[firstStateName];
    }

    public void OnEvent(UIEvent uievent)
    {
        CurState.OnEvent(uievent, this);
    }

    public void Switch(string newStateName)
    {
        CurState.OnExit?.Invoke(this);
        CurState = StateDic[newStateName];
        CurState.OnEnter?.Invoke(this);
    }
}