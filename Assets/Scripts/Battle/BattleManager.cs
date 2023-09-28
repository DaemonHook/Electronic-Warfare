/*
 * file: BattleManager.cs
 * feature: 战斗管理器
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class BattleManager : MonoBehaviour
{
    public int RowSize, ColSize; //战场大小

    public GameObject TouchPad; //触控板的prefab

    private void OnTouchpadClicked(int row, int col)
    {
        Debug.Log($"{row}, {col} clicked!");
    }

    private void CreateTouchPads()
    {
        for (int i = 0; i < RowSize; i++)
        {
            for (int j = 0; j < ColSize; j++)
            {
                var go = GameObject.Instantiate(TouchPad, new Vector3((float)i, (float)j, 0f), Quaternion.identity,
                    transform);
                int ti = i, tj = j;
                go.GetComponent<TouchPad>().Init(
                    (PointerEventData ped) => { OnTouchpadClicked(ti, tj); },
                    i, j);
            }
        }
    }

    private void Awake()
    {
        CreateTouchPads();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}