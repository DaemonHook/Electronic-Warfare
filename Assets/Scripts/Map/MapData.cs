/*
 * file: MapData.cs
 * author: D.H.
 * feature: 地图数据结构
 */
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.XPath;
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

public enum Layers
{
    Terrain,
    Object,
    Unit
}

/// <summary>
/// Tiled地图原始数据
/// </summary>
public class TiledMap
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    /// <summary>
    /// 图块集名称
    /// </summary>
    public List<string> TileSets { get; private set; }

    struct Tile
    {
        public int setId;  // 图块集id
        public int tileId; // 图块id
    }

    Tile[,] Terrain, Object, Unit;

    public (string, int) GetObject(Vector2Int pos, Layers layer)
    {
        switch (layer)
        {
            case Layers.Terrain:
                return (TileSets[Terrain[pos.x, pos.y].setId], Terrain[pos.x, pos.y].tileId);
            case Layers.Object:
                return (TileSets[Object[pos.x, pos.y].setId], Terrain[pos.x, pos.y].tileId);
            case Layers.Unit:
                return (T)
            default:
                return ("", -1);
        }
    }

    public TiledMap(string raw)
    {
        XPathDocument doc = new XPathDocument(new StringReader(raw));

    }
}
