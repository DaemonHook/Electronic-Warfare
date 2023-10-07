using System;

/// <summary>
/// UI事件类型
/// </summary>
public enum UIEventType
{
    Click,    
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
}