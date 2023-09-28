/*
 * file: MapData.cs
 * author: D.H.
 * feature: 地图数据结构
 */

using System;
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

public enum Layers
{
    Terrain,
    Object,
    Unit
}

/// <summary>
/// Tiled 编辑器中的瓦片 命名为tiled，避免与unity内置组件混淆
/// </summary>
public class Tiled
{
    public string setName; // 图块集名称
    public int tileId; // 图块在图块集中的id

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
                    LoadLayer(Terrain, splited);
                    break;
                case "Unit":
                    LoadLayer(Unit, splited);
                    break;
                case "Object":
                    LoadLayer(Object, splited);
                    break;
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

#region 全局属性定义

/// <summary>
/// 瓦片在游戏中的类型
/// </summary>
public enum TileType
{
    Terrain,
    Object,
    Unit
}

/// <summary>
/// 一个mod内瓦片的总集合，包含id, tile类型，sprite名称
/// </summary>
public class TileSet
{
    private Dictionary<int, (TileType, string)> dic;

    /// <summary>
    /// 为集合添加瓦片定义
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="type">tile类型</param>
    /// <param name="spriteName">sprite名称</param>
    public void AddTile(int id, TileType type, string spriteName)
    {
        dic.Add(id, (type, spriteName));
    }

    public TileType GetTileType(int id)
    {
        return dic[id].Item1;
    }

    public string GetSpriteName(int id)
    {
        return dic[id].Item2;
    }

    public TileSet()
    {
        dic = new Dictionary<int, (TileType, string)>();
    }
}

/// <summary>
/// 地形种类
/// </summary>
public enum TerrainType
{
    Ground, //平地
    Water, //水面
    Road, //公路
    Wood //林地
}


/// <summary>
/// 单位类型
/// </summary>
public enum UnitType
{
    Building,
    Army,
}

/// <summary>
/// 攻击类型
/// </summary>
public enum AttackType
{
    None,
    Light,
    Heavy,
}

/// <summary>
/// 全局单位数据定义 定义在Unit.csv中
/// </summary>
public class UnitDefine
{
    public int team; //队伍
    public string name; //名称（游戏内）
    public UnitType type; //单位类型

    //属性 分别为生命，移动点数，攻击范围，攻击力
    public int hp, mp, sight, atkRange, atk;

    //攻击类型
    public AttackType attackType;

    public UnitDefine(int team, string name, UnitType type, int hp, int mp, int sight, int atkRange, int atk,
        AttackType attackType)
    {
        this.team = team;
        this.name = name;
        this.type = type;
        this.hp = hp;
        this.mp = mp;
        this.sight = sight;
        this.atkRange = atkRange;
        this.atk = atk;
        this.attackType = attackType;
    }

    /// <summary>
    /// 从配置文件的一行生成一个UnitDefine
    /// </summary>
    /// <param name="line">CSVDocument的一行</param>
    public static UnitDefine LoadUnitDefine(Dictionary<string, string> line)
    {
        int team = int.Parse(line["team"]);
        string name = line["name"];
        UnitType utype = Enum.Parse<UnitType>(line["type"], true);
        int hp = int.Parse(line["hp"]);
        int mp = int.Parse(line["mp"]);
        int sight = int.Parse(line["sight"]);
        int atkRange = int.Parse(line["range"]);
        int atk = int.Parse(line["atk"]);
        AttackType attackType = Enum.Parse<AttackType>(line["attack_type"]);
        return new UnitDefine(team, name, utype, hp, mp, sight, atkRange, atk, attackType);
    }
}

#endregion

/// <summary>
/// 游戏包（mod）
/// </summary>
public class Package
{
    public string Name { get; private set; }
    public Dictionary<int, GameObject> Prefabs;
    public Dictionary<int, UnitDefine> UnitDefines;
    public Dictionary<int, TerrainType> TerrainTypes;

    public Package(string name)
    {
        Name = name;
        Prefabs = new Dictionary<int, GameObject>();
        UnitDefines = new Dictionary<int, UnitDefine>();
    }
}