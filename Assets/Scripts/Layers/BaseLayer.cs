using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 图层控件
/// </summary>
public class BaseLayer : MonoBehaviour
{
    /// <summary>
    /// 图层的子物体
    /// </summary>
    public Dictionary<(int, int), GameObject> SubObjects { get; private set; }

    public virtual void AddObject(int x, int y, GameObject obj)
    {
        if (SubObjects.ContainsKey((x, y)))
        {
            Debug.LogError($"duplicated object: {(x, y)}");
        }
        else
        {
            SubObjects.Add((x, y), obj);
        }
    }

    /// <summary>
    /// 移除对应位置的子物体
    /// </summary>
    public virtual void RemoveObject(int x, int y)
    {
        if (!SubObjects.ContainsKey((x, y)))
        {
            Debug.LogError($"no object at {(x, y)}");
        }
        else
        {
            SubObjects.Remove((x, y));
        }
    }

    /// <summary>
    /// 移除子物体
    /// </summary>
    public virtual void RemoveObject(GameObject obj)
    {
        var item = SubObjects.First(kvp => kvp.Value == obj);
        SubObjects.Remove(item.Key);
    }

    /// <summary>
    /// 重新设定位置
    /// </summary>
    public virtual void RelocateObject(int oldx, int oldy, int newx, int newy)
    {
        if (SubObjects.ContainsKey((newx, newy)) || !SubObjects.ContainsKey((oldx, oldy)))
        {
            Debug.LogError($"Conflict relocation: {(oldx, oldy)} to {(newx, newy)}");
        }
        else
        {
            GameObject go = SubObjects[(oldx, oldy)];
            SubObjects.Remove((oldx, oldy));
            SubObjects.Add((newx, newy), go);
        }
    }

    /// <summary>
    /// 重新设定物体位置
    /// </summary>
    public virtual void RelocateObject(GameObject obj, int newx, int newy)
    {
        var item = SubObjects.First((kvp => kvp.Value == obj));
        SubObjects.Remove(item.Key);
        SubObjects.Add((newx, newy), item.Value);
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
