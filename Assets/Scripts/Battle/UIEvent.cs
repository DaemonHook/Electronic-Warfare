using System;
using UnityEngine.SubsystemsImplementation;

/// <summary>
/// UI事件类型
/// </summary>
[Serializable]
public enum UIEventType
{
    Click,
    Select,
}

/// <summary>
/// UI事件
/// </summary>
[Serializable]
public class UIEvent
{
    public UIEventType Type;

    public object Param;

    public UIEvent(UIEventType type, object param)
    {
        Type = type;
        Param = param;
    }

    public override string ToString()
    {
        return $"UIEvent: type: [{Type}] param: [{Param}]";
    }

    public Type GetParamType()
    {
        switch (Param)
        {
            case UIEventType.Click:
                return (1,1).GetType();
            default:
                return null;
        }
    }
}