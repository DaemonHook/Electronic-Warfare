using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 视图全局管理器
/// </summary>
public static class ViewManager
{
    // view预制体的缓存
    static Dictionary<string, GameObject> viewCache = new Dictionary<string, GameObject>();
    
    static ViewManager()
    {
        var baseViews = Resources.LoadAll<GameObject>("View");
        foreach (var view in baseViews)
        {
            if (view.GetComponent<BaseView>() != null)
            {
                viewCache.Add(view.gameObject.name, view);
            }
        }
    }

    public static GameObject CreateView(string name, Transform parent)
    {
        if (!viewCache.ContainsKey(name))
        {
            Debug.LogError($"Error creating view named {name}");
            return null;
        }
        var go = GameObject.Instantiate(viewCache[name], parent);
        return go;
    }
}

public static class GameApp
{
    static List<BaseView> openViews;    
}
