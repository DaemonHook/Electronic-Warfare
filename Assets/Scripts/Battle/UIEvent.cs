using System;
using UnityEngine.SubsystemsImplementation;

/// <summary>
/// UI事件类型
/// </summary>
[Serializable]
public enum UIEventType
{
    Click,      // 点击地图上的格子
    // Select,
    Confirm,    // 点击确认按钮
    NextTurn,   // 点击下一回合
    ThisTurn,   // 轮到当前玩家行动
}

/// <summary>
/// UI事件
/// </summary>
[Serializable]
public class UIEvent
{
    public UIEventType Type;

    public object[] Params;

    public UIEvent(UIEventType type, params object[] @params)
    {
        Type = type;
        Params = @params;
    }

    public override string ToString()
    {
        return $"UIEvent: type: [{Type}] param: [{Params}]";
    }
}