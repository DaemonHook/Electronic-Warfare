using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 确定行动
/// </summary>
public class ConfirmPad : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        BattleManager.Instance.ConfirmOperation();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("pointerdown at " + (Row, Col));
    }
}