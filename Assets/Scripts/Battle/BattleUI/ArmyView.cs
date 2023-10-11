using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 左上面板之上
/// </summary>
public class ArmyView : MonoBehaviour
{
    public Image ArmyImage;
    public Text NameText, TeamText;
    public Text HPText, AttackText, OilText, MPText;

    public void Refresh(Sprite armyImage, string nameStr, string team, string hp, string attack,
        string oil, string mp)
    {
        ArmyImage.sprite = armyImage;
        NameText.text = nameStr;
        TeamText.text = "隊伍：" + team;
        HPText.text = hp;
        AttackText.text = attack;
        OilText.text = oil;
        MPText.text = mp;
    }
}