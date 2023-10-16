using System;
using UnityEngine;

public class UnitTile : GameTile
{
    public UnitProperty OriginProperty;
    public UnitProperty CurrentProperty;
    
    private HPPanel hpPanel;
    
    public void Init(UnitProperty property, int x, int y)
    {
        GameTile:Init(x, y);
        hpPanel = GetComponentInChildren<HPPanel>();
        OriginProperty = property.Clone();
        CurrentProperty = property.Clone();
        RefreshDisplay();
    }
    
    /// <summary>
    /// 此单位是否为活跃单位（当前操作的单位）
    /// </summary>
    private bool active = false;
    
    protected override void ReceiveUIEvent(UIEvent uiEvent)
    {
        base.ReceiveUIEvent(uiEvent);
        switch (uiEvent.Type)
        {
            case UIEventType.Click:
                var (x, y) = ((int, int))uiEvent.Param;
                if ((x, y) == (PosX, PosY))
                {
                    active = !active;
                    if (active)
                    {}
                }
                break;
            default:
                break;
        }
    }

    public void RefreshDisplay()
    {
        hpPanel.SetHPDisplay((int)Math.Round(((double)CurrentProperty.hp / OriginProperty.hp) * 10));
    }

    
}