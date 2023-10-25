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
    /// <summary>
    /// 获取战斗事件的类型所对应的参数类型
    /// </summary>
    /// <param name="battleEventType">战斗事件类型</param>
    /// <param name="index">参数索引</param>
    /// <returns>参数类型</returns>
    public static Type GetBattleEventParamsType(BattleEventType battleEventType, int index)
    {
        return battleEventType switch
        {
            BattleEventType.Attack =>
                //TODO 添加类型
                null,
            BattleEventType.Move => index switch
            {
                0 => typeof(ValueTuple<int, int>),
                1 => typeof(ValueTuple<int, int>),
                _ => null
            },
            _ => null
        };
    }

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