using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    public Transform LeftTopTr;
    public GameObject LTPanel;

    public Transform RightTopTr;
    
    public Transform RightBottomTr;
    
    public ArmyView armyView;
    public TerrainView terrainView;

    public Text CurTeamText;
    
    public void Awake()
    {
        Instance = this;
        transform.Find("LeftTop").position = new Vector3(Screen.safeArea.xMin, Screen.safeArea.yMax);
        transform.Find("RightTop").position = new Vector3(Screen.safeArea.xMax, Screen.safeArea.yMax);
        transform.Find("RightBottom").position = new Vector3(Screen.safeArea.xMax, Screen.safeArea.yMin);
    }

    private void Start()
    {
        BattleManager.Instance.RegisterUIEventHandler(OnUIEvent);
        BattleManager.Instance.RegisterBattleEventHandler(OnBattleEvent);
        LTPanel.SetActive(false);
    }

    private void SetArmyViewActive(bool on)
    {
        armyView.gameObject.SetActive(on);
    }

    private ValueTuple<int, int> lastClick = (-666, -100866);

    private void OnBattleEvent(BattleEvent battleEvent)
    {
        switch (battleEvent.Type)
        {
            case BattleEventType.Move:
                break;
            case BattleEventType.Attack:
                break;
            default:
                break;
        }
    }
    
    private void OnUIEvent(UIEvent uiEvent)
    {
        switch (uiEvent.Type)
        {
            case UIEventType.Click:
                var cord = (Vector2Int)uiEvent.Params[0];
                int i = cord.x;
                int j = cord.y;
                if (lastClick == (i, j))
                {
                    LTPanel.SetActive(false);
                    lastClick = (-9879, -524389);
                    break;
                }
                LTPanel.SetActive(true);
                lastClick = (i, j);
                GameObject terrainGO = BattleManager.Instance.Terrains[i, j]?.gameObject;
                GameObject objectGO = BattleManager.Instance.Objects[i, j]?.gameObject;
                GameObject unitGO = BattleManager.Instance.Units[i, j]?.gameObject;

                Sprite terrainSprite = null;
                if (terrainGO != null)
                {
                    terrainSprite = terrainGO.GetComponent<SpriteRenderer>().sprite;
                }
                
                terrainView.SetTerrainImage(terrainSprite);


                Sprite objectSprite = null;
                if (objectGO != null)
                {
                    objectSprite = objectGO.GetComponent<SpriteRenderer>().sprite;
                }

                terrainView.SetObjectImage(objectSprite);
                Debug.Log($"objectGo: {objectGO} objectSprite: {objectSprite}");
                if (unitGO != null)
                {
                    Debug.Log($"show unit info at {i}, {j}");
                    SetArmyViewActive(true);
                    UnitTile unitTile = unitGO.GetComponent<UnitTile>();
                    UnitProperty prop = unitTile.CurrentProperty;
                    var unitSprite = unitGO.GetComponent<SpriteRenderer>().sprite;
                    armyView.Refresh(unitSprite, prop.name, prop.team.ToString(), prop.hp.ToString(),
                        prop.atk.ToString(), "--", prop.mp.ToString());
                }
                else
                {
                    SetArmyViewActive(false);
                }
                break;
            case UIEventType.Confirm:
                break;
            case UIEventType.NextTurn:
                int nextTeam = (int)uiEvent.Params[0];
                Debug.Log($"更新队伍为：{BattleManager.TeamColorStrings[nextTeam]}");
                CurTeamText.text = $"当前行动：{BattleManager.TeamColorStrings[nextTeam]}";
                break;
            default:
                break;
        }
    }
}