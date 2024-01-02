using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 可以进行的操作状态
/// </summary>
public enum ControlState
{
    None, //无（或选择）
    Moveable, //可移动  
    Attackable, //可攻击
    Buildable, //可建造
}

/// <summary>
/// 触控板
/// </summary>
public class TouchPad : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public int Row, Col;

    public Color MovableColor, AttackableColor;
    public Color SelectedColor;
    public Color BuildableColor;

    private GameObject SelectGO;

    private void Awake()
    {
        SelectGO = transform.Find("Select").gameObject;
        BattleManager.Instance.RegisterUIEventHandler(ReceiveUIEvent);
    }

    public void Init(int row, int col)
    {
        Row = row;
        Col = col;
        SetControlState(ControlState.None);
    }

    private void ReceiveUIEvent(UIEvent uiEvent)
    {
        switch (uiEvent.Type)
        {
            case UIEventType.Click:
                Vector2Int clickedCord = (Vector2Int)uiEvent.Params[0];
                int x = clickedCord.x;
                int y = clickedCord.y;
                if (x == Row && y == Col)
                {
                    SelectGO.SetActive(true);
                }
                else
                {
                    SelectGO.SetActive(false);
                }

                break;
            default:
                break;
        }
    }


    #region 控制显示样式

    public SpriteRenderer ControlSpriteRenderer;

    public void SetControlState(ControlState state)
    {
        // Debug.Log($"Touchpad at {Row}, {Col} state set to {state}");
        switch (state)
        {
            case ControlState.None:
                ControlSpriteRenderer.color = Color.clear;
                break;
            case ControlState.Moveable:
                // Debug.Log($"TouchPad at {Row}, {Col} set control state: {state}");
                ControlSpriteRenderer.color = MovableColor;
                break;
            case ControlState.Attackable:
                ControlSpriteRenderer.color = AttackableColor;
                break;
            case ControlState.Buildable:
                ControlSpriteRenderer.color = BuildableColor;
                break;
            default: break;
        }
    }

    #endregion

    public void OnPointerUp(PointerEventData eventData)
    {
        BattleManager.Instance.OnTouchpadClicked(Row, Col);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}