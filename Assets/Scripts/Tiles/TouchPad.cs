/*
 * file: TouchPad.cs
 * feature: 检测对于棋盘格子的点击
 */

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
public enum TouchState
{
    None, //无（或选择）
    Moveable, //可移动  
    Attackable, //可攻击
    Buildable, //可建造
}

/// <summary>
/// 触控板
/// </summary>
public class TouchPad : MonoBehaviour, IPointerClickHandler
{
    public Action<PointerEventData> callbackOnClick;
    public int Row, Col;

    public Color MovableColor, AttackableColor;
    public Color SelectedColor;

    private GameObject SelectGO;

    private void Awake()
    {
        SelectGO = transform.Find("Select").gameObject;
    }

    public void Init(Action<PointerEventData> callBack, int row, int col)
    {
        callbackOnClick = callBack;
        Row = row;
        Col = col;
        BattleManager.Instance.RegisterUIEventHandler(ReceiveUIEvent);
    }

    private bool selected = false;

    private void OnClicked()
    {
        selected = !selected;
        SelectGO.SetActive(selected);
    }

    private void ReceiveUIEvent(UIEvent uiEvent)
    {
        switch (uiEvent.Type)
        {
            case UIEventType.Click:
                var (x, y) = ((int, int))uiEvent.Param;

                if (x == Row && y == Col)
                {
                    OnClicked();
                }
                else
                {
                    selected = false;
                    SelectGO.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"touchpad at {(Row, Col)} is clicked!");
        callbackOnClick?.Invoke(eventData);
        // throw new NotImplementedException();
    }

    private void ChangeColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}