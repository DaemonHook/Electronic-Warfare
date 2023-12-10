/*
 * file: BattleManager.cs
 * feature: 战斗管理器
 */

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
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
    public int Height; //战场大小
    public int Width; //战场大小

    public Vector2Int BattleFieldSize
    {
        get { return new Vector2Int(Width, Height); }
    }


    /// <summary>
    /// 将逻辑上的单位位置转化为实际的世界位置
    /// </summary>
    /// <param name="logicPosition"></param>
    /// <returns></returns>
    public Vector3 GetRealWorldPosition(Vector2Int logicPosition)
    {
        return new Vector3(logicPosition.x, logicPosition.y, 0);
    }

    public GameObject TouchPad; //触控板的prefab
    public Package Package { get; private set; } //当前加载的package

    public TerrainTile[,] Terrains;
    public ObjectTile[,] Objects;
    public UnitTile[,] Units;

    public TouchPad[,] TouchPads;

    private void CreateTouchPads()
    {
        TouchPads = new TouchPad[Width, Height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var go = GameObject.Instantiate(TouchPad, new Vector3(i, j, 0f), Quaternion.identity,
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
        RegisterBattleEventHandler(ReceiveBattleEvent);
        RegisterUIEventHandler(ReceiveUIEvent);

        // 第一个回合开始
        AddBattleEvent(new BattleEvent(BattleEventType.NextTurn, 1));
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
                if ((Units[i, j].CurrentProperty.team) != -1)
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
                }
            }
        }
    }

    private void CreateTerrains()
    {
        Terrains = new TerrainTile[Width, Height];
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
                    Terrains[i, j] = go.GetComponent<TerrainTile>();
                }
            }
        }
    }

    #region 队伍系统

    // TODO: 支持多玩家
    public int PlayerTeam = 0;
    public int CurrentTeam { get; set; } = 1; //当前行动的队伍
    public int TeamCount;

    public Dictionary<int, List<UnitTile>> TeamUnits;

    public static string[] TeamColorStrings =
    {
        "白色",
        "藍色",
        "黃色",
    };

    /// <summary>
    /// 玩家点击下一回合按键
    /// </summary>
    public void OnNextTurnButton()
    {
        Debug.Log($"OnNextTurnButton");
        if (CurrentTeam == PlayerTeam)
        {
            AddBattleEvent(new BattleEvent(BattleEventType.NextTurn, (CurrentTeam + 1) % TeamCount));
        }
    }

    #endregion

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

    /// <summary>
    /// 添加单位行动事件
    /// </summary>
    /// <param name="unit">行动的单位</param>
    /// <param name="type">行动类型</param>
    /// <param name="param">行动参数</param>
    public void AddUnitOperation(UnitTile unit, OperationType type, params object[] param)
    {
        print($"DoOperation: operation type {type} to {param[0]}");
        switch (type)
        {
            case OperationType.Attack:
                //TODO add
                break;
            case OperationType.Move:
                Vector2Int target = (Vector2Int)param[0];
                List<Vector2Int> path = (List<Vector2Int>)param[1];
                AddBattleEvent(new BattleEvent(BattleEventType.Move, unit.LogicPosition, target, path));
                break;
        }
    }

    private void FixedUpdate()
    {
        HandleBattleEvent();
        HandleUIEvent();
    }

    /// <summary>
    /// 为了逻辑上的完备和将来重构的方便，BattleManager也会监听战斗事件
    /// </summary>
    public void ReceiveBattleEvent(BattleEvent battleEvent)
    {
        Debug.Log($"Received Battle Event: {{type: {battleEvent.Type}, params: {battleEvent.Params}}}");
        switch (battleEvent.Type)
        {
            case BattleEventType.Move:
                Vector2Int startPos = (Vector2Int)battleEvent.Params[0];
                Vector2Int endPos = (Vector2Int)battleEvent.Params[1];
                if (Units[endPos.x, endPos.y] != null)
                    throw new Exception("duplicated unit position");
                Units[endPos.x, endPos.y] = Units[startPos.x, startPos.y];
                Units[startPos.x, startPos.y] = null;
                Debug.Log($"unit at {startPos} move to {endPos}");
                break;
            case BattleEventType.Attack:
                break;
            case BattleEventType.NextTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ReceiveUIEvent(UIEvent uiEvent)
    {
        switch (uiEvent.Type)
        {
            case UIEventType.Click:
                break;
            case UIEventType.Select:
                UnitTile tile = ((UnitTile)uiEvent.Params[0]);
                DisplayUnitOperations(tile);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion


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

    //  当前操作状态
    private OperatingState curOperatingState = OperatingState.NoSelect;

    /// <summary>
    /// 当前待确定的操作发生的位置
    /// </summary>
    private Vector2Int curOperationPos;

    public void OnTouchpadClicked(int row, int col)
    {
        print($"Touchpad at {row}, {col} clicked");
        //在更新之后，unit tile不再处理点击事件，以便后续更加灵活的状态机实现
        AddUIEvent(new UIEvent(UIEventType.Click, (row, col)));

        curOperationPos = new Vector2Int(row, col);

        UnitTile curClickedUnit = Units[row, col];
        if (curClickedUnit != null)
        {
            Debug.Log($"curClickedUnit: {curClickedUnit}");
        }

        Debug.Log($"curOperatingState: {curOperatingState}");
        switch (curOperatingState)
        {
            case OperatingState.NoSelect:
                if (curClickedUnit != null)
                {
                    //显示单位状态和行动范围
                    DisplayUnitOperations(curClickedUnit);
                    if (curClickedUnit.CurrentProperty.team == CurrentTeam)
                    {
                        Debug.Log($"entered Select Mode");
                        //如果点击的是自己的单位，那么就进入Selected状态
                        curOperatingState = OperatingState.Selected;
                    }
                }

                break;
            case OperatingState.Selected:
                if (curActiveOperations.ContainsKey(new(row, col)))
                {
                    confirmPad = Instantiate(confirmPadPrefab,
                        new Vector3(row, col + 1), Quaternion.identity);
                    curOperatingState = OperatingState.Confirming;
                }
                else
                {
                    if (curClickedUnit == curActiveUnit)
                    {
                        DisplayUnitOperations(null);
                        curOperatingState = OperatingState.NoSelect;
                    }
                    else
                    {
                        DisplayUnitOperations(curClickedUnit);
                        if (curClickedUnit != null && curClickedUnit.CurrentProperty.team != CurrentTeam)
                        {
                            curOperatingState = OperatingState.NoSelect;
                        }
                    }
                }

                break;
            case OperatingState.Confirming:
                // TODO: 这里逻辑不好，要重写
                if (curClickedUnit != null && curClickedUnit != curActiveUnit)
                {
                    curOperatingState = OperatingState.NoSelect;
                    //显示单位状态和行动范围
                    DisplayUnitOperations(curClickedUnit);
                    if (curClickedUnit.CurrentProperty.team == CurrentTeam)
                    {
                        Debug.Log($"entered Select Mode");
                        //如果点击的是自己的单位，那么就进入Selected状态
                        curOperatingState = OperatingState.Selected;
                    }
                }
                else
                {
                    Destroy(confirmPad);
                    confirmPad = null;
                    curOperatingState = OperatingState.Selected;
                    if (curActiveOperations.ContainsKey(new(row, col)))
                    {
                        confirmPad = Instantiate(confirmPadPrefab,
                            new Vector3(row, col + 1), Quaternion.identity);
                        curOperatingState = OperatingState.Confirming;
                    }
                }

                break;
        }
    }

    public GameObject confirmPadPrefab;
    private GameObject confirmPad; //行动确认按钮

    /// <summary>
    /// 由确定按钮调用，确定当前选中的操作
    /// </summary>
    public void ConfirmOperation()
    {
        var curOperationType = curActiveOperations[curOperationPos];
        switch (curOperationType)
        {
            case OperationType.Attack:
                throw new Exception("尚未实现");
            case OperationType.Move:

                // 获取移动路径
                PathFinder.PathNode curNode = curUnitMovableNodes.Find(node => node.Equals(curOperationPos));
                if (curNode == null)
                    throw new Exception("curNode not in curUnitMovableNodes");
                List<Vector2Int> path = new List<Vector2Int>();

                while (curNode != null)
                {
                    path.Add(new(curNode.x, curNode.y));
                    curNode = curNode.preNode;
                }

                // 在路径中删除单位当前位置
                path.RemoveAt(path.Count - 1);

                // 节点为倒序排列，所以需要将其反向
                path.Reverse();

                string s = "";
                foreach (var n in path)
                {
                    s += n.ToString() + ' ';
                }

                Debug.Log("path: " + s);

                AddUnitOperation(curActiveUnit, OperationType.Move, curOperationPos, path);
                break;
        }

        AddUIEvent(new UIEvent(UIEventType.Select, curActiveUnit));
    }

    /// <summary>
    /// 现在活跃的单位
    /// </summary>
    private UnitTile curActiveUnit;

    /// <summary>
    /// 现在活跃的单位可以移动的节点
    /// </summary>
    private List<PathFinder.PathNode> curUnitMovableNodes;

    private Dictionary<Vector2Int, OperationType> curActiveOperations = new();

    private bool UnitCanPass(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return false;
        ObjectTile objectTile = Objects[posX, posY];
        TerrainTile terrainTile = Terrains[posX, posY];
        return curActiveUnit.CanPass(objectTile, terrainTile);
    }


    /// <summary>
    /// 显示当前选中的单位可执行的行动（若为null则为取消选择）
    /// </summary>
    private void DisplayUnitOperations(UnitTile unitTile)
    {
        curActiveUnit = null;
        ClearOpeartionsDisplay();
        curActiveOperations.Clear();

        if (unitTile != null)
        {
            // string tmp = "";

            // 刷新可行动作

            // 添加移动动作
            curActiveUnit = unitTile;
            if (curActiveUnit.CanMove)
            {
                Debug.Log($"cur unit can move.");
                curUnitMovableNodes = PathFinder.Find(unitTile.PosX, unitTile.PosY,
                    // 单位可以通过地形且该格子无其他单位
                    unitTile.CurrentProperty.mp, (x, y) =>
                    {
                        if (UnitCanPass(x, y))
                        {
                            return Units[x, y] == null;
                        }

                        return false;
                    });
                foreach (var node in curUnitMovableNodes)
                {
                    curActiveOperations.Add(new(node.x, node.y), OperationType.Move);
                }
            }
            else
            {
                Debug.Log("cur unit can not move.");
            }

            if (curActiveUnit.CanAttack)
            {
            }
            //TODO：加入其他动作
        }


        DisplayActiveOperations();
    }

    private void ClearOpeartionsDisplay()
    {
        foreach (var p in curActiveOperations.Keys)
        {
            TouchPads[p.x, p.y].SetControlState(ControlState.None);
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
                    TouchPads[pos.x, pos.y].SetControlState(ControlState.Moveable);
                    break;
                case OperationType.Attack:
                    break;
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
            DisplayUnitOperations(null);
        }
        else
        {
            DisplayUnitOperations(unitTile);
        }
    }

    #endregion
}