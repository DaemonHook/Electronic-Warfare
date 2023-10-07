using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    public Transform LTPanelTr;
    
    public void Awake()
    {
        Instance = this;
        LTPanelTr = transform.Find("LeftTop/LTPanel");
        transform.Find("LeftTop").position = new Vector3(Screen.safeArea.xMin, Screen.safeArea.yMax);
    }

    public void SwitchLTPanel(bool on)
    {
        LTPanelTr.gameObject.SetActive(on);
    }

    public void SwitchArmyView(bool on)
    {
        LTPanelTr.Find("ArmyView").gameObject.SetActive(on);
    }

    public void ShowArmyData(Sprite armySprite, int HP, int MP, int attack, int oil)
    {
        LTPanelTr.Find("ArmyView/ArmyImage").GetComponent<SpriteRenderer>().sprite = armySprite;
        LTPanelTr.Find("ArmyView/HPText").GetComponent<Text>().text = HP.ToString();
 
        LTPanelTr.Find("ArmyView/MPText").GetComponent<Text>().text = MP.ToString();
        LTPanelTr.Find("ArmyView/AttackText").GetComponent<Text>().text = attack.ToString();
        LTPanelTr.Find("ArmyView/HPText").GetComponent<Text>().text = HP.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
