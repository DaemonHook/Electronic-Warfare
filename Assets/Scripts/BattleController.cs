using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 游戏界面的管理
/// </summary>
public class Battle
{
    /// <summary>
    /// 游戏逻辑坐标转化至实际的世界坐标
    /// </summary>
    /// <param name="cord"></param>
    /// <returns></returns>
    public Vector3 Cord2WorldPos(Vector2Int cord)
    {
        return new Vector3(cord.x, cord.y, 0);
    }
}