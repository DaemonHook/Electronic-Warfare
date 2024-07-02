using System;
using System.Collections;
using System.IO;
using System.Resources;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Test
{
    public class MyClass
    {
        public void PrintClassName()
        {
            Console.WriteLine("Class name: " + typeof(MyClass).Name);
        }
    }
    [TestFixture]
    public class TestTest
    {

        [Test]
        public void Test()
        {
            //            // public字段都是单位属性
            //public int team;        // 队伍
            //    public string name;     // 名称（游戏内）
            //    public UnitType type;   // 单位类型（建筑或部队）
            //    public int hp,          // 血量
            //               mp,          // 移动点数
            //               sight,       // 视野距离
            //               atkRange,    // 攻击距离
            //               atk;         // 攻击力
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"team",  "1"},
                {"name", "newbee" },
                { "type", "building" }
            };
            UnitProperty prop = new UnitProperty(dict);
            Debug.Log(prop.ToString());
        }
    }
}