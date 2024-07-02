using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


/// <summary>
/// 地形和物体种类
/// </summary>
public enum BlockType
{
    Ground,     // 平地
    Water,      // 水面
    Hill,       // 山地
    Road,       // 公路
    Wood,       // 林地
    Block,      // 障碍
    Building,   // 建筑
    Unit        // 单位
}


/// <summary>
/// 单位类型
/// </summary>
public enum UnitType
{
    Building,
    Army,
}

/// <summary>
/// 攻击类型
/// </summary>
[Serializable]
public enum AttackType
{
    None,
    Light,
    Heavy,
}

[Serializable]
public enum MovementType
{
    Air,
    Ground,
    Water
}


/// <summary>
/// 单位属性
/// </summary>
[Serializable]
public class UnitProperty
{
    // public字段都是单位属性
    public int team;        // 队伍
    public string name;     // 名称（游戏内）
    public UnitType type;   // 单位类型（建筑或部队）
    public int hp,          // 血量
               mp,          // 移动点数
               sight,       // 视野距离
               atkRange,    // 攻击距离
               atk;         // 攻击力

    public UnitProperty(IDictionary<string, string> properties)
    {
        Type type = typeof(UnitProperty);

        // 获取所有public字段
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            if (properties.ContainsKey(field.Name))
            {
                field.SetValue(this, ConvertToType(field.FieldType,
                    properties[field.Name]));
            }
        }
    }

    public override string ToString()
    {
        Type udType = this.GetType();
        StringBuilder sb = new StringBuilder();
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (FieldInfo field in fields)
        {
            object fieldValue = field.GetValue(this);
            sb.Append($"{field.Name}: {fieldValue}\t");
        }

        return sb.ToString();
    }

    static object ConvertToType(Type type, string stringValue)
    {
        if (type == typeof(int))
        {
            return int.Parse(stringValue);
        }
        else if (type == typeof(string))
        {
            return stringValue;
        }
        else if (type == typeof(double))
        {
            return double.Parse(stringValue);
        }
        else if (type.IsEnum)
        {
            return Enum.Parse(type, stringValue, true);
        }
        else
        {
            throw new NotSupportedException($"Type {type} is not supported");
        }
    }
}
