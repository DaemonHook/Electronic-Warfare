using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitTile : GameTile
{
    public UnitProperty OriginProperty;
    public UnitProperty CurrentProperty;
    public bool ThisTurnMoved { get; private set; }
    public bool ThisTurnAttacked { get; private set; }
    [Header("移动动画的速度")] public float MoveAniSpeed;
    private HPPanel hpPanel;

    public override string ToString()
    {
        return $"{CurrentProperty.name} at {(PosX, PosY)} team {CurrentProperty.team}";
    }

    public void Init(UnitProperty property, int x, int y)
    {
        Init(x, y);
        hpPanel = GetComponentInChildren<HPPanel>();
        OriginProperty = property.Clone();
        CurrentProperty = property.Clone();
        RefreshDisplay();
    }

    // protected override void ReceiveUIEvent(UIEvent uiEvent)
    // {
    //     base.ReceiveUIEvent(uiEvent);
    //     switch (uiEvent.Type)
    //     {
    //         case UIEventType.Click:
    //             var (x, y) = ((int, int))uiEvent.Param;
    //             if ((x, y) == (PosX, PosY))
    //             {
    //                 BattleManager.Instance.OnUnitClick(this);
    //             }
    //
    //             break;
    //         default:
    //             break;
    //     }
    // }

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
        if (CurrentProperty.movementType == MovementType.Air) return true;
        if (objectTile != null)
        {
            switch (objectTile.GetBlockType())
            {
                case BlockType.Road: return true;
                case BlockType.Water:
                    if (CurrentProperty.movementType != MovementType.Water) return false;
                    break;
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
                case BlockType.Water:
                    if (CurrentProperty.movementType != MovementType.Water) return false;
                    break;
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

    private void FaceToRight()
    {
        transform.localScale = new(1.0f, 1.0f, 1.0f);
    }

    private void FaceToLeft()
    {
        transform.localScale = new(-1.0f, 1.0f, 1.0f);
    }

    private Vector2Int curTarget;

    private void MoveByPath(List<Vector2Int> path)
    {
        curTarget = LogicPosition;
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.OnUpdate(() =>
        {
            if (((int)transform.position.x) < curTarget.x)
            {
                FaceToRight();
            }
            else if ((int)transform.position.x > curTarget.x)
            {
                FaceToLeft();
            }
        });
        foreach (var pos in path)
        {
            Tween move = transform.DOMove(new Vector3(pos.x, pos.y), Setting.SpeedPerTile);
            move.OnStart(() => { curTarget = pos; });
        }

        moveSequence.Play();
    }

    protected override void ReceiveBattleEvent(BattleEvent battleEvent)
    {
        switch (battleEvent.Type)
        {
            case BattleEventType.Attack:
                //TODO
                break;
            case BattleEventType.Move:

                Vector2Int startPos = (Vector2Int)battleEvent.Params[0];
                if (startPos != LogicPosition) break;
                Vector2Int endPos = ((Vector2Int)battleEvent.Params[1]);
                List<Vector2Int> path = ((List<Vector2Int>)battleEvent.Params[2]);
                MoveByPath(path);
                LogicPosition = endPos;
                Debug.Log($"{name} move to {endPos}");
                break;
        }
    }
}