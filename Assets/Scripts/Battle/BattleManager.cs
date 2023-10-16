/*
 * file: BattleManager.cs
 * feature: 战斗管理器
 */

using System;
using System.Collections.Generic;
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
    public static BattleManager Instance;

    public string PackageName;
    public string MapName;

    public TiledMap Map { get; set; }
    [FormerlySerializedAs("RowSize")] public int Height; //战场大小
    [FormerlySerializedAs("ColSize")] public int Width; //战场大小

    public GameObject TouchPad; //触控板的prefab
    public Package Package { get; private set; } //当前加载的package

    public TerrainTile[,] Terrains;
    public GameObject[,] TerrainGOs;
    public ObjectTile[,] Objects;
    public GameObject[,] ObjectGOs;
    public UnitTile[,] Units;
    public GameObject[,] UnitGOs;

    private void OnTouchpadClicked(int row, int col)
    {
        AddUIEvent(new UIEvent(UIEventType.Click, (row, col)));
    }

    private void CreateTouchPads()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
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
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
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
        Debug.Log($"map height: {Height}, width: {Width}");
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
        MovableCamera.Instance.Init(Width, Height, Width / 2, Height / 2);
    }

    private void CreateUnits()
    {
        Units = new UnitTile[Width, Height];
        UnitGOs = new GameObject[Width, Height];
        var unitIds = Map.Unit;
        TeamCount = 0;
        TeamUnits = new Dictionary<int, List<UnitTile>>();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (unitIds[i, j] == null) continue;
                var prefab = Package.Prefabs[unitIds[i, j].tileId];
                var go = Factory.Instance.UnitFactory(prefab, i, j, Package.UnitProperties[unitIds[i, j].tileId]);
                Units[i, j] = go.GetComponent<UnitTile>();
                UnitGOs[i, j] = go;
                int t;
                if ((t = Units[i, j].CurrentProperty.team) != -1)
                {
                    if (TeamUnits.ContainsKey(Units[i, j].CurrentProperty.team))
                    {
                        TeamUnits[i].Add(Units[i, j]);
                    }
                }
            }
        }
    }

    private void CreateObjects()
    {
        Objects = new ObjectTile[Width, Height];
        ObjectGOs = new GameObject[Width, Height];
        var objectIds = Map.Object;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (objectIds[i, j] != null)
                {
                    var prefab = Package.Prefabs[objectIds[i, j].tileId];
                    var go = Factory.Instance.ObjectFactory(prefab, i, j, Package.TerrainTypes[objectIds[i, j].tileId]);
                    Objects[i, j] = go.GetComponent<ObjectTile>();
                    ObjectGOs[i, j] = go;
                }
            }
        }
    }

    private void CreateTerrains()
    {
        Terrains = new TerrainTile[Width, Height];
        TerrainGOs = new GameObject[Width, Height];
        var terrainIds = Map.Terrain;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (terrainIds[i, j] != null)
                {
                    var prefab = Package.Prefabs[terrainIds[i, j].tileId];
                    var go = Factory.Instance.TerrainFactory(prefab, i, j,
                        Package.TerrainTypes[terrainIds[i, j].tileId]);
                    TerrainGOs[i, j] = go;
                    Terrains[i, j] = go.GetComponent<TerrainTile>();
                }
            }
        }
    }

    public int TeamCount;

    public Dictionary<int, List<UnitTile>> TeamUnits;


    #region 事件处理

    private Queue<UIEvent> uiEventQueue = new Queue<UIEvent>();
    private Queue<BattleEvent> battleEventQueue = new Queue<BattleEvent>();

    private event Action<UIEvent> registeredUIHandlers;
    private event Action<BattleEvent> registeredBattleHandlers;

    public void RegisterUIEventHandler(Action<UIEvent> action)
    {
        registeredUIHandlers += action;
    }

    public void UnregisterUIEventHandler(Action<UIEvent> action)
    {
        registeredUIHandlers -= action;
    }

    public void RegisterBattleEventHandler(Action<BattleEvent> action)
    {
        registeredBattleHandlers += action;
    }

    public void UnregisterBattleEventHandler(Action<BattleEvent> action)
    {
        registeredBattleHandlers -= action;
    }

    public void AddUIEvent(UIEvent uiEvent)
    {
        uiEventQueue.Enqueue(uiEvent);
    }

    public void AddBattleEvent(BattleEvent battleEvent)
    {
        battleEventQueue.Enqueue(battleEvent);
    }

    private void HandleUIEvent()
    {
        while (uiEventQueue.Count > 0)
        {
            var uiEvent = uiEventQueue.Dequeue();
            Debug.Log($"current ui event: {uiEvent}");
            registeredUIHandlers?.Invoke(uiEvent);
        }
    }

    private void HandleBattleEvent()
    {
        while (battleEventQueue.Count > 0)
        {
            var battleEvent = battleEventQueue.Dequeue();
            Debug.Log($"current battle event: {battleEvent}");
            registeredBattleHandlers?.Invoke(battleEvent);
        }
    }

    private void FixedUpdate()
    {
        HandleBattleEvent();
        HandleUIEvent();
    }

    #endregion

    #region 单位操作

    private UnitTile curActiveUnit = null;

    private Dictionary<ValueTuple<int, int>, OperationType> curActiveOperations =
        new Dictionary<(int, int), OperationType>();
    
    
    /// <summary>
    /// 对于选中的单位，显示其可行操作，若重复选择，则视为取消选择
    /// </summary>
    public void UnitSelected(UnitTile unitTile)
    {
        if (curActiveUnit == unitTile)
        {
            
        }
    }

    #endregion
}