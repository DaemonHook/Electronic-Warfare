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
    private GameObject ControlGO;

    private void Awake()
    {
        SelectGO = transform.Find("Select").gameObject;
        ControlGO = transform.Find("Control").gameObject;
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
                var (x, y) = ((int, int))uiEvent.Param;

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

    private void ChangeColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    #region 控制显示样式

    public void SetControlState(ControlState state)
    {
        var controlSp = ControlGO.GetComponent<SpriteRenderer>();
        switch (state)
        {
            case ControlState.None:
                controlSp.color = Color.clear;
                break;
            case ControlState.Moveable:
                controlSp.color = MovableColor;
                break;
            case ControlState.Attackable:
                controlSp.color = AttackableColor;
                break;
            case ControlState.Buildable:
                controlSp.color = BuildableColor;
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