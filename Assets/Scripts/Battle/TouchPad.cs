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
/// 触控板
/// </summary>
public class TouchPad : MonoBehaviour, IPointerClickHandler
{
    public Action<PointerEventData> callbackOnClick;
    public int Row, Col;
    public void Init(Action<PointerEventData> callBack, int row, int col)
    {
        callbackOnClick = callBack;
        Row = row;
        Col = col;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"touchpad at {(Row, Col)} is clicked!");
        callbackOnClick?.Invoke(eventData);
        // throw new NotImplementedException();
    }
}