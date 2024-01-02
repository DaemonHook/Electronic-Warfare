using System;
using UnityEngine;

/// <summary>
/// 游戏中的瓦片（即格子）上的单位
/// </summary>
public class GameTile : MonoBehaviour
{
    public virtual BlockType GetBlockType()
    {
        return BlockType.Block;
    }
    
    /// <summary>
    /// x坐标，与transform实际坐标相同
    /// </summary>
    public int PosX { get; private set; }
    
    /// <summary>
    /// y坐标，与transform实际坐标相同
    /// </summary>
    public int PosY { get; private set; }

    public Vector2Int LogicPosition
    {
        get => new(PosX, PosY);
        protected set
        {
            PosX = value.x;
            PosY = value.y;
        }
    }
    
    public virtual void Init(int x, int y)
    {
        PosX = x;
        PosY = y;
        transform.position = new Vector3(((float)x), ((float)y));
    }

    protected virtual void ReceiveUIEvent(UIEvent uiEvent)
    {
        
    }

    protected virtual void ReceiveBattleEvent(BattleEvent battleEvent)
    {
        
    }

    private void Awake()
    {
        BattleManager.Instance.RegisterUIEventHandler(ReceiveUIEvent);
        BattleManager.Instance.RegisterBattleEventHandler(ReceiveBattleEvent);
    }

    private void OnDestroy()
    {
        BattleManager.Instance.UnregisterUIEventHandler(ReceiveUIEvent);
        BattleManager.Instance.UnregisterBattleEventHandler(ReceiveBattleEvent);
    }
}