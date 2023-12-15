using System;
using UnityEngine.SubsystemsImplementation;

/// <summary>
/// UI事件类型
/// </summary>
[Serializable]
public enum UIEventType
{
    Click,
    // Select,
    Confirm,
    Refresh,
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