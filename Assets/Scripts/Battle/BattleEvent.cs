
using System;

/// <summary>
/// 战场事件类型
/// </summary>
[Serializable]
public enum BattleEventType
{
    Move,       //移动
    Attack,     //攻击
}

/// <summary>
/// 战斗事件
/// </summary>
[Serializable]
public class BattleEvent
{
    public BattleEventType Type;
    public object Param;

    public BattleEvent(BattleEventType type, object param)
    {
        Type = type;
        Param = param;
    }

    public override string ToString()
    {
        return $"BattleEvent: type: [{Type}] param: [{Param}]";
    }
}