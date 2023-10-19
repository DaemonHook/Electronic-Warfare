/*
 * file: BattleManager.cs
 * feature: 战斗管理器
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public TouchPad[,] TouchPads;

    private void OnTouchpadClicked(int row, int col)
    {
        AddUIEvent(new UIEvent(UIEventType.Click, (row, col)));
    }

    private void CreateTouchPads()
    {
        TouchPads = new TouchPad[Width, Height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var go = GameObject.Instantiate(TouchPad, new Vector3((float)i, (float)j, 0f), Quaternion.identity,
                    transform);
                go.transform.SetParent(GameObject.Find("Manager/TouchPads").transform);
                go.GetComponent<SpriteRenderer>().sortingOrder = LayerOrders.TouchPad;
                int ti = i, tj = j;
                TouchPads[i, j] = go.GetComponent<TouchPad>();
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
        Package = new Package(GameApp.PackageName);
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
            //Debug.Log($"current ui event: {uiEvent}");
            registeredUIHandlers?.Invoke(uiEvent);
        }
    }

    private void HandleBattleEvent()
    {
        while (battleEventQueue.Count > 0)
        {
            var battleEvent = battleEventQueue.Dequeue();
            //Debug.Log($"current battle event: {battleEvent}");
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

    /// <summary>
    /// 现在活跃的单位
    /// </summary>
    private UnitTile curActiveUnit = null;

    private Dictionary<ValueTuple<int, int>, OperationType> curActiveOperations =
        new Dictionary<(int, int), OperationType>();

    private bool UnitCanPass(int posX, int posY)
    {
        ObjectTile objectTile = Objects[posX, posY];
        TerrainTile terrainTile = Terrains[posX, posY];
        return curActiveUnit.CanPass(objectTile, terrainTile);
    }

    /// <summary>
    /// 设置当前选中的单位（若为null则为取消选择）
    /// </summary>
    private void SetActiveUnit(UnitTile unitTile)
    {
        curActiveUnit = null;
        ClearOpeartionsDisplay();
        curActiveOperations.Clear();
        if (unitTile != null)
        {
            // 刷新可行动作

            // 添加移动动作
            curActiveUnit = unitTile;
            var curList = PathFinder.Find(unitTile.PosX, unitTile.PosY,
                // 单位可以通过地形且该格子无其他单位
                unitTile.CurrentProperty.mp, (int x, int y) =>
                {
                    if (UnitCanPass(x, y))
                    {
                        return Units[x, y] == null;
                    }
                    return false;
                });
            foreach (var node in curList)
            {
                //print(node);
                curActiveOperations.Add((node.x, node.y), OperationType.Move);
            }

            //TODO：加入其他动作
        }

        DisplayActiveOperations();
    }

    private void ClearOpeartionsDisplay()
    {
        foreach (var (i, j) in curActiveOperations.Keys)
        {
            TouchPads[i, j].SetControlState(ControlState.None);
        }
    }

    private void DisplayActiveOperations()
    {
        foreach (var (pos, type) in curActiveOperations)
        {
            //TODO 添加其他动作显示
            switch (type)
            {
                case OperationType.Move:
                    TouchPads[pos.Item1, pos.Item2].SetControlState(ControlState.Moveable);
                    break;
                case OperationType.Attack:
                    break;
                default: break;
            }
        }
    }

    /// <summary>
    /// 对于选中的单位，显示其可行操作，若重复选择，则视为取消
    /// </summary>
    public void UnitClick(UnitTile unitTile)
    {
        Debug.Log($"UnitClick at {unitTile.PosX}, {unitTile.PosY}");

        if (curActiveUnit == unitTile)
        {
            SetActiveUnit(null);
        }
        else
        {
            SetActiveUnit(unitTile);
        }
    }

    #endregion
}