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
using System.Xml;
using System.Xml.XPath;
using UnityEditor.Rendering.Universal;
using UnityEngine;

/// <summary>
/// 在tileset的Define文件中对瓦片的定义
/// </summary>
public class BlockDefine
{
    public int id;
    public string sprite_name,
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
/// 瓦片 命名为tiled，避免与unity内置组件混淆
/// </summary>
public class Tiled
{
    public string setName;  // 图块集名称
    public int tileId;      // 图块在图块集中的id
    public Tiled(string name, int tileId)
    {
        this.setName = name;
        this.tileId = tileId;
    }

    public override string ToString()
    {
        return $"setName: {setName}, tileId: {tileId}";
    }
}

/// <summary>
/// Tiled地图原始数据
/// </summary>
public class TiledMap
{
    #region 外部接口
    public int Width { get; private set; }
    public int Height { get; private set; }

    /// <summary>
    /// 图块集
    /// item1：图块集名称 item2：图块集初始gid
    /// </summary>
    public List<(string, int)> TileSets { get; private set; }

    public Tiled[,] Terrain, Object, Unit;

    /// <summary>
    /// 获取地图图块信息
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="layer">层级</param>
    /// <returns>(图集名称, 图块编号)</returns>
    public Tiled GetObject(Vector2Int pos, Layers layer)
    {
        switch (layer)
        {
            case Layers.Terrain:
                return Terrain[pos.x, pos.y];
            case Layers.Object:
                return Object[pos.x, pos.y];
            case Layers.Unit:
                return Unit[pos.x, pos.y];
            default: return null;
        }
    }

    #endregion


    private XmlDocument doc;

    private Tiled GetTiledByGid(int gid)
    {
        if (gid == 0)
        {
            return null;
        }
        int i;
        for (i = 0; i < TileSets.Count - 1; i++)
        {
            if (TileSets[i + 1].Item2 > gid)
            {
                break;
            }
        }
        return new Tiled(TileSets[i].Item1, gid - TileSets[i].Item2);
    }


    public TiledMap(string raw)
    {
        doc = new XmlDocument();
        doc.LoadXml(raw);

        // 获取地图基本信息
        var m = doc.SelectSingleNode("/map");
        Width = int.Parse(m.Attributes["width"].Value);
        Height = int.Parse(m.Attributes["height"].Value);

        // 初始化成员
        Terrain = new Tiled[Width, Height];
        Object = new Tiled[Width, Height];
        Unit = new Tiled[Width, Height];
        TileSets = new List<(string, int)>();

        // 读取图块集信息
        var tilesets = doc.SelectNodes("/map/tileset");
        foreach (XmlNode tileset in tilesets)
        {
            var source = tileset.Attributes["source"].Value;
            // 这里需要删除后缀名
            if (source.LastIndexOf(".") != -1)
                source = source.Remove(source.LastIndexOf("."));
            if (source.LastIndexOf("/") != -1)
                source = source.Substring(source.LastIndexOf("/") + 1);
            var firstGid = int.Parse(tileset.Attributes["firstgid"].Value);
            TileSets.Add((source, firstGid));
        }
        // 以firstgid排序
        TileSets.Sort((x, y) => { return x.Item2.CompareTo(y.Item2); });

        // 读取图层内容
        var layers = doc.SelectNodes("/map/layer");
        foreach (XmlNode layer in layers)
        {
            var layerName = layer.Attributes["name"].Value;
            var content = layer.InnerText;
            var splited = content.Split(",");
            switch (layerName)
            {
                case "Terrain":
                    LoadLayer(Terrain, splited); break;
                case "Unit":
                    LoadLayer(Unit, splited); break;
                case "Object":
                    LoadLayer(Object, splited); break;
                default: break;
            }
        }
    }

    private void LoadLayer(Tiled[,] layer, string[] content)
    {
        for (int i = 0; i < content.Length; i++)
        {
            int x = i % Width;
            int y = Height - 1 - (i / Width);
            long TILED_FILP_CONST = 1073741824; // Tiled的翻转常数
            layer[x, y] = GetTiledByGid((int)(long.Parse(content[i]) % TILED_FILP_CONST));
        }
    }
}

/// <summary>
/// 
/// </summary>
public class Package
{

    public Package(string name)
    {

    }
}