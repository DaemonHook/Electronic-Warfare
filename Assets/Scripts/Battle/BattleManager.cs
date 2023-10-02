/*
 * file: BattleManager.cs
 * feature: 战斗管理器
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public static class LayerOrders
{
    public static int
        Terrain = 0,
        Object = 10,
        Unit = 20,
        TouchPad = 30;
}

public class BattleManager : MonoBehaviour
{
    public string PackageName;
    public string MapName;

    public TiledMap Map { get; set; }
    [FormerlySerializedAs("RowSize")] public int Height; //战场大小
    [FormerlySerializedAs("ColSize")] public int Width; //战场大小

    public GameObject TouchPad; //触控板的prefab
    public Package Package { get; private set; } //当前加载的package

    public TerrainTile[,] Terrains;
    public ObjectTile[,] Objects;
    public UnitTile[,] Units;

    private void OnTouchpadClicked(int row, int col)
    {
        Debug.Log($"{row}, {col} clicked!");
    }

    private void CreateTouchPads()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                var go = GameObject.Instantiate(TouchPad, new Vector3((float)i, (float)j, 0f), Quaternion.identity,
                    transform);
                go.transform.SetParent(GameObject.Find("Manager/TouchPads").transform);
                go.GetComponent<SpriteRenderer>().sortingOrder = LayerOrders.TouchPad;
                int ti = i, tj = j;
                go.GetComponent<TouchPad>().Init(
                    (PointerEventData ped) => { OnTouchpadClicked(ti, tj); },
                    i, j);
            }
        }
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        LoadPackage();
        LoadMap();
        ApplySettings();
        CreateTouchPads();
        CreateBattleField();
    }

    private void LoadMap()
    {
        Map = Package.Maps[MapName];
        Height = Map.Height;
        Width = Map.Width;
    }


    private void CreateBattleField()
    {
        CreateTerrains();
        CreateObjects();
        CreateUnits();
    }

    private void LoadPackage()
    {
        Package = new Package(PackageName);
    }

    private void ApplySettings()
    {
        // throw new System.NotImplementedException();
    }

    private void CreateUnits()
    {
        // throw new System.NotImplementedException();
    }

    private void CreateObjects()
    {
        // throw new System.NotImplementedException();
    }

    private void CreateTerrains()
    {
        Terrains = new TerrainTile[Width, Height];
        
    }
}