using UnityEditor.TerrainTools;
using UnityEngine;

/// <summary>
/// 动态创建游戏对象
/// </summary>
public static class Factory
{
    public static Transform TerrainParent = null;
    
    public static GameObject TerrainFactory(GameObject prefab, int x, int y, TerrainType terrainType)
    {
        var newGO = GameObject.Instantiate(prefab, TerrainParent);
        var tt = newGO.AddComponent<TerrainTile>();
        tt.Init(x, y, terrainType);
        return newGO;
    }
}