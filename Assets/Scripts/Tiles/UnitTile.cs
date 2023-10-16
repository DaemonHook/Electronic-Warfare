using System;
using UnityEngine;

public class UnitTile : GameTile
{
    public UnitProperty OriginProperty;
    public UnitProperty CurrentProperty;
    public bool ThisTurnMoved;
    public bool ThisTurnAttacked;
    
    private HPPanel hpPanel;
    
    public void Init(UnitProperty property, int x, int y)
    {
        GameTile:Init(x, y);
        hpPanel = GetComponentInChildren<HPPanel>();
        OriginProperty = property.Clone();
        CurrentProperty = property.Clone();
        RefreshDisplay();
    }
    
    protected override void ReceiveUIEvent(UIEvent uiEvent)
    {
        base.ReceiveUIEvent(uiEvent);
        switch (uiEvent.Type)
        {
            case UIEventType.Click:
                var (x, y) = ((int, int))uiEvent.Param;
                if ((x, y) == (PosX, PosY))
                {
                    
                }
                break;
            default:
                break;
        }
    }

    public bool Selected { get; set; }
    
    public void RefreshDisplay()
    {
        hpPanel.SetHPDisplay((int)Math.Round(((double)CurrentProperty.hp / OriginProperty.hp) * 10));
    }

    
}