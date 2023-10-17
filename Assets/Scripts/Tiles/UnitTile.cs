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
        Init(x, y);
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
                    BattleManager.Instance.UnitClick(this);
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

    /// <summary>
    /// 决定单位是否可以经过一个格子
    /// </summary>
    public bool CanPass(ObjectTile objectTile, TerrainTile terrainTile)
    {
        if (objectTile != null)
        {
            switch (objectTile.GetBlockType())
            {
                case BlockType.Road: return true;
                case BlockType.Water: return false;
                case BlockType.Ground: break;
                case BlockType.Hill: return false;
                case BlockType.Block: return false;
                case BlockType.Unit: return false;
                case BlockType.Wood:
                    break;
                case BlockType.Building:
                    break;
                default: break;
            }
        }

        if (terrainTile != null)
        {
            switch (terrainTile.GetBlockType())
            {
                case BlockType.Road: return true;
                case BlockType.Water: return false;
                case BlockType.Ground: break;
                case BlockType.Hill: return false;
                case BlockType.Block: return false;
                case BlockType.Unit: return false;
                case BlockType.Wood:
                    break;
                case BlockType.Building:
                    break;
                default: break;
            }
        }

        return true;
    }
}