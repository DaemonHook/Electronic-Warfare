/*
 * file: MapLoader.cs
 * author: D.H.
 * feature: Tiled地图读取
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using UnityEngine;

public class MapLoader
{
    #region tiled相关常数
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
    #endregion

    #region 内部变量
    string mapRawText;
    XmlDocument xmlDoc;
    int width, height;
    #endregion

    /// <summary>
    /// 图层内块的表示
    /// </summary>
    struct InnerBlock
    {
        /// <summary>
        /// 图块集名称
        /// </summary>
        public string tilesetName;
        /// <summary>
        /// 图块集内的id
        /// </summary>
        public int id;
    }

    class TileSet
    {
        public int firstgid;

    }

    #region 接口
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    #endregion
}