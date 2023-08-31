/*
 * file: MapData.cs
 * author: D.H.
 * feature: 地图数据结构
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在tileset的Define文件中对瓦片的定义
/// </summary>
[SerializeField]
public class BlockDefine
{
    public int id;
    public string   sprite_name,
                    type,
                    team,
                    name,
                    hp,
                    mp,
                    armor,
                    sight,
                    range,
                    atk_to_light,
                    atk_to_heavy,
                    terrain_type,
                    building_type,
                    cost,
                    special;
}

[SerializeField]
public class UnitData
{
}

/// <summary>
/// 地图信息
/// </summary>
[SerializeField]
public class MapData
{
    public int Version;

}
