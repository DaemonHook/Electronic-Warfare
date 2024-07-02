using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// 瓦片在游戏中的类型
/// </summary>
public enum TileType
{
    Terrain,
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
    private void AddTile(int id, TileType type, string spriteName)
    {
        dic.Add(id, (type, spriteName));
    }

    public List<int> GetValidIds()
    {
        return new List<int>(dic.Keys);
    }

    public TileType GetTileType(int id)
    {
        return dic[id].Item1;
    }

    public string GetSpriteName(int id)
    {
        return dic[id].Item2;
    }

    /// <summary>
    /// 根据CSVDocument的行创建TileSet
    /// </summary>
    /// <param name="csvDocumentLines"></param>
    public TileSet(List<Dictionary<string, string>> csvDocumentLines)
    {
        dic = new Dictionary<int, (TileType, string)>();
        foreach (var line in csvDocumentLines)
        {
            int id = int.Parse(line["id"]);
            TileType type = Enum.Parse<TileType>(line["type"], true);
            string spriteName = line["tile_name"];
            AddTile(id, type, spriteName);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("tileSet:\n");
        foreach (var tile in dic)
        {
            sb.Append($"{tile.Key}\t{tile.Value.Item1}\t{tile.Value.Item2}\n");
        }

        return sb.ToString();
    }
}
