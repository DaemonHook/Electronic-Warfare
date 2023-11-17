using System;
using System.Collections.Generic;

/// <summary>
/// 战场事件类型
/// </summary>
[Serializable]
public enum BattleEventType
{
    Move, //移动
    Attack, //攻击
}

/// <summary>
/// 战斗事件
/// </summary>
[Serializable]
public class BattleEvent
{
    public BattleEventType Type;
    public object[] Params;

    public BattleEvent(BattleEventType type, params object[] prms)
    {
        Type = type;
        Params = prms;
    }

    public override string ToString()
    {
        return $"BattleEvent: type: [{Type}] param: [{Params}]";
    }
}