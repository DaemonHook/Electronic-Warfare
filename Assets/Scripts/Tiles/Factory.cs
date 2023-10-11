using System;
using UnityEngine;

/// <summary>
/// 动态创建游戏对象
/// </summary>
public class Factory : MonoBehaviour
{
    public static Factory Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform TerrainParent;

    public Transform ObjectParent;

    public Transform UnitParent;

    /// <summary>
    /// 单位HP面板之prefab
    /// </summary>
    public GameObject HpPrefab;

    public GameObject TerrainFactory(GameObject prefab, int x, int y, TerrainType terrainType)
    {
        var newGO = GameObject.Instantiate(prefab, TerrainParent);
        var tt = newGO.AddComponent<TerrainTile>();
        tt.Init(x, y, terrainType);
        newGO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
        newGO.SetActive(true);
        newGO.tag = "Terrain";
        return newGO;
    }

    public GameObject ObjectFactory(GameObject prefab, int x, int y, TerrainType terrainType)
    {
        var newGO = GameObject.Instantiate(prefab, ObjectParent);
        var tt = newGO.AddComponent<TerrainTile>();
        tt.Init(x, y, terrainType);
        newGO.GetComponent<SpriteRenderer>().sortingLayerName = "Object";
        newGO.SetActive(true);
        newGO.tag = "Object";
        return newGO;
    }

    public GameObject UnitFactory(GameObject prefab, int x, int y, UnitProperty property)
    {
        var newGO = GameObject.Instantiate(prefab, UnitParent);
        var newHpGO = Instantiate(HpPrefab, newGO.transform);
        var unitTile = newGO.AddComponent<UnitTile>();
        unitTile.Init(property, x, y);
        newGO.GetComponent<SpriteRenderer>().sortingLayerName = "Unit";
        newGO.SetActive(true);
        newGO.tag = "Unit";
        return newGO;
    }
}