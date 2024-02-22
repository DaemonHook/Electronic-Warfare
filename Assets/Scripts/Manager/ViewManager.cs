using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewManager : BaseManager<ViewManager>
{
    // 现在打开的 view
    private List<BaseView> openedViews = new();
    // canvas Transform
    private Transform canvasTr;

    /// <summary>
    /// 从 Resource 中读取 view 并打开
    /// </summary>
    /// <param name="viewName"></param>
    public void OpenView(string viewName)
    {
        Resources.Load<GameObject>(viewName);
    }
    
    /// <summary>
    /// 开启 View
    /// </summary>
    /// <param name="view"></param>
    public void OpenView(BaseView view)
    {
        view.Open();
        openedViews.Add(view);
        view.transform.SetParent(transform);
    }

    /// <summary>
    /// 关闭 View
    /// </summary>
    /// <param name="index"></param>
    public void CloseView(int index)
    {
        BaseView view = openedViews[index];
        openedViews.RemoveAt(index);
        view.Close();
    }

    public void CloseView(string handle)
    {
        BaseView view = openedViews.First(baseView => baseView.GetHandle() == handle);
        openedViews.Remove(view);
        view.Close();
    }
}