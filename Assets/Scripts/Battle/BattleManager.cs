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

    [FormerlySerializedAs("PackageName")] public string ModuleName;
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
                TouchPads[i, j] = go.GetComponent<TouchPad>();
                go.GetComponent<TouchPad>().Init(i, j);
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

    private void GetMapName()
    {
        if (GameApp.ModuleName != null)
        {
            ModuleName = GameApp.ModuleName;
        }

        if (GameApp.MapName != null)
        {
            MapName = GameApp.MapName;
        }
    }

    private void Init()
    {
        GetMapName();
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
        Package = new Package(ModuleName);
    }

    private void ApplySettings()
    {
        BattleCamera.Instance.Init(Width, Height, Width / 2, Height / 2);
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
    
    /// <summary>
    /// ui事件队列
    /// </summary>
    private Queue<UIEvent> uiEventQueue = new Queue<UIEvent>();
    
    /// <summary>
    /// 战斗事件队列
    /// </summary>
    private Queue<BattleEvent> battleEventQueue = new Queue<BattleEvent>();
    
    /// <summary>
    /// 注册的UI事件监听者
    /// </summary>
    private event Action<UIEvent> registeredUIHandlers;
    
    /// <summary>
    /// 注册的战斗事件监听者
    /// </summary>
    private event Action<BattleEvent> registeredBattleHandlers;
    
    /// <summary>
    /// 监听ui事件
    /// </summary>
    /// <param name="action"></param>
    public void RegisterUIEventHandler(Action<UIEvent> action)
    {
        registeredUIHandlers += action;
    }
    
    /// <summary>
    /// 取消监听ui事件
    /// </summary>
    /// <param name="action"></param>
    public void UnregisterUIEventHandler(Action<UIEvent> action)
    {
        registeredUIHandlers -= action;
    }
    
    /// <summary>
    /// 监听战斗事件
    /// </summary>
    /// <param name="action"></param>
    public void RegisterBattleEventHandler(Action<BattleEvent> action)
    {
        registeredBattleHandlers += action;
    }
    
    /// <summary>
    /// 取消监听战斗事件
    /// </summary>
    /// <param name="action"></param>
    public void UnregisterBattleEventHandler(Action<BattleEvent> action)
    {
        registeredBattleHandlers -= action;
    }
    
    /// <summary>
    /// 产生ui事件
    /// </summary>
    /// <param name="uiEvent"></param>
    public void AddUIEvent(UIEvent uiEvent)
    {
        uiEventQueue.Enqueue(uiEvent);
    }
    
    /// <summary>
    /// 产生战斗事件
    /// </summary>
    /// <param name="battleEvent"></param>
    public void AddBattleEvent(BattleEvent battleEvent)
    {
        battleEventQueue.Enqueue(battleEvent);
    }
    
    /// <summary>
    /// 分发ui事件
    /// </summary>
    private void HandleUIEvent()
    {
        while (uiEventQueue.Count > 0)
        {
            var uiEvent = uiEventQueue.Dequeue();
            //Debug.Log($"current ui event: {uiEvent}");
            registeredUIHandlers?.Invoke(uiEvent);
        }
    }
    
    /// <summary>
    /// 分发战斗事件
    /// </summary>
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
        NextActionAfterEvents.Invoke();
        NextActionAfterEvents = () => { };
    }

    /// <summary>
    /// 下一帧在事件之后执行的动作（只执行一次）
    /// </summary>
    public Action NextActionAfterEvents = () => { };

    #endregion

    public int CurTeam { get; set; } = 1; //当前行动的队伍

    #region 玩家操作处理

    /// <summary>
    /// 玩家的操作状态
    /// </summary>
    private enum OperatingState
    {
        NoSelect, //没有选中单位
        Selected, //已经选中单位，但未指定操作
        Confirming, //指定了操作，确认中 
    }

    //当前操作状态
    private OperatingState curOperatingState = OperatingState.NoSelect;

    /// <summary>
    /// 当前有待确定的操作（即操作确认按钮所确认的工作）
    /// </summary>
    private ValueTuple<int, int> curConfirmingOpeartionPos;

    public void OnTouchpadClicked(int row, int col)
    {
        print($"Touchpad at {row}, {col} clicked");
        //在更新之后，unit tile不再处理点击事件，以便后续更加灵活的状态机实现
        AddUIEvent(new UIEvent(UIEventType.Click, (row, col)));

        UnitTile curClickedUnit = Units[row, col];
        Debug.Log($"curClickedUnit: {curClickedUnit}");
        switch (curOperatingState)
        {
            case OperatingState.NoSelect:
                if (curClickedUnit != null)
                {
                    //显示单位状态和行动范围
                    SetActiveUnit(curClickedUnit);
                    if (curClickedUnit.CurrentProperty.team == CurTeam)
                    {
                        //如果点击的是自己的单位，那么就进入Selected状态
                        curOperatingState = OperatingState.Selected;
                    }
                }

                break;
            case OperatingState.Selected:
                if (curActiveOperations.ContainsKey((row, col)))
                {
                    confirmPad = Instantiate(confirmPadPrefab,
                        new Vector3(row, col + 1), Quaternion.identity);
                    curOperatingState = OperatingState.Confirming;
                    curConfirmingOpeartionPos = (row, col);
                }
                else
                {
                    if (curClickedUnit == curActiveUnit)
                    {
                        SetActiveUnit(null);
                        curOperatingState = OperatingState.NoSelect;
                    }
                    else
                    {
                        SetActiveUnit(curClickedUnit);
                        if (curClickedUnit != null && curClickedUnit.CurrentProperty.team != CurTeam)
                        {
                            curOperatingState = OperatingState.NoSelect;
                        }
                    }
                }

                break;
            case OperatingState.Confirming:
                Destroy(confirmPad);
                confirmPad = null;
                curOperatingState = OperatingState.Selected;
                if (curActiveOperations.ContainsKey((row, col)))
                {
                    confirmPad = Instantiate(confirmPadPrefab,
                        new Vector3(row, col + 1), Quaternion.identity);
                    curOperatingState = OperatingState.Confirming;
                }

                break;
            default: break;
        }
    }

    public GameObject confirmPadPrefab;
    private GameObject confirmPad; //行动确认按钮

    /// <summary>
    /// 由确定按钮调用，确定当前选中的操作
    /// </summary>
    public void ConfirmOperation()
    {
        AddOperation(curActiveUnit, curActiveOperations[curConfirmingOpeartionPos], curConfirmingOpeartionPos);
        // curOperatingState = OperatingState.NoSelect;
        // SetActiveUnit(null);
        NextActionAfterEvents += () => { SetActiveUnit(curActiveUnit); };
    }

    /// <summary>
    /// 现在活跃的单位
    /// </summary>
    private UnitTile curActiveUnit = null;

    private Dictionary<ValueTuple<int, int>, OperationType> curActiveOperations =
        new Dictionary<(int, int), OperationType>();

    private bool UnitCanPass(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return false;
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
    /// 点击单位事件的处理
    /// </summary>
    public void OnUnitClick(UnitTile unitTile)
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

    #region 单位行动系统
    
    /// <summary>
    /// 添加单位行动事件
    /// </summary>
    /// <param name="unit">行动的单位</param>
    /// <param name="type">行动类型</param>
    /// <param name="param">行动参数</param>
    public void AddOperation(UnitTile unit, OperationType type, params object[] param)
    {
        print($"DoOperation: operation type {type} to {param[0]}");
        switch (type)
        {
            case OperationType.Attack:
                //TODO add
                break;
            case OperationType.Move:
                ValueTuple<int, int> target = (ValueTuple<int, int>)param[0];
                
                break;
            default:
                break;
        }
    }

    #endregion
}