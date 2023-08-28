using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapLoader
{
    
    public readonly int FILP_OFFSET = 1073741824;   //tiled的翻转常数
    public readonly int HORIZONTAL = 1;             //水平翻转
    public readonly int VERTICAL = 2;               //垂直翻转
    public readonly int HORIZONTAL_AND_VERTICAL = 3;//水平+垂直翻转

    /*
     * Tiled反转计算方式：
     * filp = gid / FILP_OFFSET
     * 0：不翻转
     * 1：水平
     * 2：垂直
     * 3：水平 + 垂直
     */

    public MapLoader(string baseUrl = "SamplePackage/Tiled")
    {
        var maps = Resources.LoadAll<TextAsset>(baseUrl);

    }

    
}