using UnityEngine;

/// <summary>
/// 游戏中的瓦片（即格子）上的单位
/// </summary>
public class GameTile : MonoBehaviour
{
    /// <summary>
    /// x坐标，与transform实际坐标相同
    /// </summary>
    public int CordX { get; private set; }
    
    /// <summary>
    /// y坐标，与transform实际坐标相同
    /// </summary>
    public int CordY { get; private set; }

    public virtual void Init(int x, int y)
    {
        CordX = x;
        CordY = y;
        transform.position = new Vector3(((float)x), ((float)y));
    }
}