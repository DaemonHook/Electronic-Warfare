/*
 * file: BattleManager.cs
 * feature: 战斗管理器
 */

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
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
    public int Height;  //战场大小
    public int Width;   //战场大小

    //TODO: 增加玩家数量
    public int PlayerNum = 2;   // 玩家数量
    public int CurPlayer { get; set; } = 0;   // 当前的玩家

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
        LoadPackage();
        GetMapName();
        LoadMap();
        ApplySettings();
        CreateTouchPads();
        CreateBattleField();

        InitManipulateStateMachine();
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
        TeamUnits = new Dictionary<int, List<UnitTile>>();
        for (int i = 0; i < TeamCount; i++)
        {
            TeamUnits.Add(i, new List<UnitTile>());
        }

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (unitIds[i, j] == null) continue;
                var prefab = Package.Prefabs[unitIds[i, j].tileId];
                var go = Factory.Instance.UnitFactory(prefab, i, j, Package.UnitProperties[unitIds[i, j].tileId]);
                Units[i, j] = go.GetComponent<UnitTile>();
                // -1表示中立单位
                if (Units[i, j].CurrentProperty.team != -1)
                {
                    if (TeamUnits.ContainsKey(Units[i, j].CurrentProperty.team))
                    {
                        TeamUnits[Units[i, j].CurrentProperty.team].Add(Units[i, j]);
                    }
                }
                else
                {
                    // TODO: 添加灵活的队伍处理
                    throw new Exception();
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

    public int PlayerTeam = 0;
    public int CurrentTeam { get; set; } = 1; //当前行动的队伍
    public int TeamCount { get { return PlayerNum; } }

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
            int nextTeam = (CurrentTeam + 1) % TeamCount;
            AddBattleEvent(new BattleEvent(BattleEventType.NextTurn, nextTeam));
            AddUIEvent(new UIEvent(UIEventType.NextTurn, nextTeam));
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
    /// 触发单位行动事件
    /// </summary>
    /// <param name="unit">行动的单位</param>
    /// <param name="type">行动类型</param>
    /// <param name="param">行动参数</param>
    public void TriggerUnitOperationEvent(UnitTile unit, OperationType type, params object[] param)
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
        Debug.Log($"uievent type:{uiEvent.Type}");

        foreach(var sm in playerSMs)
        {
            sm.OnEvent(uiEvent);
        }
    }

    #endregion


    #region 玩家操作处理



    private bool UnitCanPass(int posX, int posY)
    {
        if (posX < 0 || posX >= Width || posY < 0 || posY >= Height) return false;
        ObjectTile objectTile = Objects[posX, posY];
        TerrainTile terrainTile = Terrains[posX, posY];
        return activeUnit.CanPass(objectTile, terrainTile);
    }

    /// <summary>
    /// 显示单位可执行的行动（若为null则为清空显示）
    /// </summary>
    private void DisplayUnitOperations(UnitTile unitTile)
    {
        // 清除之前的显示
        foreach (var p in curActiveOperations.Keys)
        {
            TouchPads[p.x, p.y].SetControlState(ControlState.None);
        }
        curActiveOperations.Clear();

        // 获取可行动作
        if (unitTile != null)
        {
            // 刷新可行动作

            // 添加移动动作
            if (unitTile.CanMove)
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

            if (unitTile.CanAttack)
            {
                //TODO
            }
            //TODO：加入其他动作
        }

        // 显示可行动作
        foreach (var (pos, type) in curActiveOperations)
        {
            //TODO 添加其他动作显示
            switch (type)
            {
                case OperationType.Move:
                    TouchPads[pos.x, pos.y].SetControlState(ControlState.Moveable);
                    break;
                case OperationType.Attack:
                    TouchPads[pos.x, pos.y].SetControlState(ControlState.Attackable);
                    break;
            }
        }
    }


    #region 状态机实现

    /// <summary>
    /// 当前活跃的（选中的）单位
    /// </summary>
    private UnitTile activeUnit;

    /// <summary>
    /// 当前活跃的单位可以移动的节点
    /// </summary>
    private List<PathFinder.PathNode> curUnitMovableNodes;

    private Dictionary<Vector2Int, OperationType> curActiveOperations = new();

    private Vector2Int curOperationPos;

    /// <summary>
    /// 选择一个单位
    /// </summary>
    /// <param name="unitTile"></param>
    private void SelectUnit(UnitTile unitTile)
    {
        DisplayUnitOperations(null);
        activeUnit = unitTile;
        DisplayUnitOperations(activeUnit);
    }

    /*
     * 操作状态
     */
    // 未选中状态
    // 此状态中，可以选中单位，若为可操控单位，则进入Active状态
    private static ManipulateState Idle = new("Idle");

    // 活跃状态（选中了自己的单位）
    // 此状态中可以对单位发出指令
    private static ManipulateState Active = new("Active");

    // 确认操作状态
    // 此状态中确认当前行动
    private static ManipulateState Confirming = new("Confirming");

    // 其他玩家在行动状态
    private static ManipulateState Waiting = new("Waiting");

    // 所有状态的数组
    private static ManipulateState[] AllStates =
    {
        Idle, Active, Confirming, Waiting
    };

    private List<StateMachine> playerSMs;

    private UnitTile GetClickedUnit(UIEvent uievent)
    {
        Vector2Int clickCord = (Vector2Int)uievent.Params[0];
        return Units[clickCord.x, clickCord.y];
    }

    /// <summary>
    /// 状态机的初始化
    /// </summary>
    private void InitManipulateStateMachine()
    {
        /*
         * Idle状态，进入时清空行动显示
         */
        Idle.OnEnter = (StateMachine sm) =>
        {
            Debug.Log($"sm of {sm.Team} entered Idle");
            DisplayUnitOperations(null);
        };

        Idle.OnEvent = (UIEvent uievent, StateMachine sm) =>
        {
            switch (uievent.Type)
            {
                case UIEventType.Click:
                    /*
                     * Idle状态，若点击了单位则刷新活跃单位，若活跃单位属于本队伍
                     * 则进入Active模式
                     */
                    SelectUnit(GetClickedUnit(uievent));
                    if (activeUnit != null)
                    {
                        if (activeUnit.CurrentProperty.team == 
                            sm.Team)
                        {
                            sm.Switch("Active");
                        }
                    }
                    break;
                case UIEventType.Confirm:
                    throw new ArgumentOutOfRangeException();
                case UIEventType.NextTurn:
                    int nextTeam = (int)uievent.Params[0];
                    if (nextTeam != CurrentTeam)
                    {
                        sm.Switch("Waiting");
                    }
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        };

        /*
         * Active状态进入，显示活跃单位可行行动
         */
        Active.OnEnter = (StateMachine sm) =>
        {
            Debug.Log($"sm of {sm.Team} entered Active");
            DisplayUnitOperations(activeUnit);
        };

        Active.OnEvent = (UIEvent uievent, StateMachine sm) =>
        {
            switch (uievent.Type)
            {
                case UIEventType.Click:
                    Vector2Int clickCord = (Vector2Int)uievent.Params[0];
                    
                    break;
                case UIEventType.Confirm:
                    throw new ArgumentOutOfRangeException();
                case UIEventType.NextTurn:
                    int nextTeam = (int)uievent.Params[0];
                    if (nextTeam != CurrentTeam)
                    {
                        sm.Switch("Waiting");
                    }
                    break;
            }
        };




        playerSMs = new();
        for (int i = 0; i < PlayerNum; i++)
        {
            playerSMs.Add(new StateMachine(i));
            foreach (var state in AllStates)
            {
                playerSMs[i].AddNewState(state);
            }
        }


    }

    #endregion

    public void OnTouchpadClicked(int row, int col)
    {
        Debug.Log($"Touchpad at {row}, {col} is clicked.");
        // curState.Event(new Vector2Int(row, col));
        AddUIEvent(new UIEvent(UIEventType.Click, new Vector2Int(row, col)));
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

                TriggerUnitOperationEvent(activeUnit, OperationType.Move, curOperationPos, path);
                break;
        }
        AddUIEvent(new UIEvent(UIEventType.Confirm, null));
    }

    #endregion
}