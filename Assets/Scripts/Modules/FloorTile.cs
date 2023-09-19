/*
 * file: FloorTile.cs
 * author: oldball
 * feature: 地板模块，即操作单位移动时点击的模块
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 地板状态，对应不同的操作
/// </summary>
public enum FloorState
{
    Idle,
    Move,
    Attack,
}

public class FloorTile : MonoBehaviour, IPointerClickHandler
{
    private SpriteRenderer spriteRenderer;

    private GameObject selected;    // 选择框

    public Color IdleColor, MoveColor, AttackColor;

    public Action OnClicked;

    public FloorState State
    {
        get
        {
            return State;
        }
        set
        {
            switch (value)
            {
                case FloorState.Idle:
                    spriteRenderer.color = IdleColor; break;
                case FloorState.Move:
                    spriteRenderer.color = MoveColor; break;
                case FloorState.Attack:
                    spriteRenderer.color = AttackColor; break;
                default: break;
            }
            State = value;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 控制块的order为5
        spriteRenderer.sortingOrder = 5;
        State = FloorState.Idle;
    }

    public void Select()
    {
        selected.SetActive(true);
    }

    public void Unselect()
    {
        selected.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }
}
