using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 左上面板之下半部：地形面板
/// </summary>
public class TerrainView : MonoBehaviour
{
    public Image TerrainImage, ObjectImage;
    
    /// <summary>
    /// 设置地形示意图
    /// </summary>
    /// <param name="terrainImage">地形图片，null为不显示</param>
    public void SetTerrainImage(Sprite terrainImage)
    {
        if (terrainImage != null)
        {
            TerrainImage.color = Color.white;
            TerrainImage.sprite = terrainImage;
        }
        else
        {
            TerrainImage.color = Color.clear;
        }
    }
    
    /// <summary>
    /// 设置地图物体图片
    /// </summary>
    /// <param name="objectImage">物体图片，null为不显示</param>
    public void SetObjectImage(Sprite objectImage)
    {
        if (objectImage != null)
        {
            ObjectImage.color = Color.white;
            ObjectImage.sprite = objectImage;
        }
        else
        {
            ObjectImage.color = Color.clear;
        }
    }
}
