using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class UnitTile : GameTile
{
    public UnitProperty OriginProperty;
    public UnitProperty CurrentProperty;
    public bool ThisTurnMoved { get; private set; }
    public bool ThisTurnAttacked { get; private set; }
    private HPPanel hpPanel;

    public bool CanMove { get; private set; }
    public bool CanAttack { get; private set; }

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
        Vector3[] pathArr = (from node in path select new Vector3(node.x, node.y)).ToArray();

        // 由于素材特性，改变朝向会美观一点
        if (path.Last().x < transform.position.x)
        {
            FaceToLeft();
        }
        else
        {
            FaceToRight();
        }

        transform.DOPath(pathArr, Setting.SpeedPerTile);
    }

    /// <summary>
    /// 单位进行下一回合
    /// </summary>
    /// <param name="team">下一回合行动队伍</param>
    protected virtual void NextTurn(int team)
    {
        CanMove = true;
        CanAttack = true;
    }
    

    /// <summary>
    /// 刷新当前状态显示
    /// </summary>
    protected virtual void RefreshAvailableStatus()
    {
        if (CanMove)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        } 
        else
        {
            // 如果没有可攻击的单位，那么显示为灰色
            Scanner scanner = new Scanner(LogicPosition,
                    BattleManager.Instance.BattleFieldSize,
                    CurrentProperty.atkRange,
                    (Vector2Int pos) =>
                    {
                        return BattleManager.Instance.Units[pos.x, pos.y] != null &&
                         BattleManager.Instance.Units[pos.x, pos.y].CurrentProperty.team !=
                            CurrentProperty.team;
                    });
            var res = scanner.Scan();
            if (res.Count() == 0)
            {
                //Debug.Log($"unit at {LogicPosition} has no attackble.");
                GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }
    }

    protected override void ReceiveBattleEvent(BattleEvent battleEvent)
    {
        switch (battleEvent.Type)
        {
            case BattleEventType.Attack:

                break;
            case BattleEventType.Move:
                Vector2Int startPos = (Vector2Int)battleEvent.Params[0];
                if (startPos != LogicPosition) return;
                Vector2Int endPos = ((Vector2Int)battleEvent.Params[1]);
                List<Vector2Int> path = ((List<Vector2Int>)battleEvent.Params[2]);
                MoveByPath(path);
                LogicPosition = endPos;
                CanMove = false;
                RefreshAvailableStatus();
                Debug.Log($"{name} move to {endPos}");
                break;
            case BattleEventType.NextTurn:
                int team = ((int)battleEvent.Params[0]);
                NextTurn(team);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}