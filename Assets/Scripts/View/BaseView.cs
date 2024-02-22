using UnityEngine;

/// <summary>
/// 所有 View 所实现的接口
/// </summary>
public abstract class BaseView : MonoBehaviour
{
    /// <summary>
    /// 开启 View，并初始化
    /// </summary>
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭 View，并销毁 GameObject
    /// </summary>
    public virtual void Close()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 显示 View
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏 View，不销毁
    /// </summary>
    public virtual void Hide()
    {
    }

    /// <summary>
    /// 获取其 GameObject 的唯一名称（即句柄）
    /// </summary>
    public virtual string GetHandle()
    {
        return $"{gameObject.name}_{gameObject.GetInstanceID()}";
    }

    /// <summary>
    /// 关闭自己
    /// </summary>
    public virtual void CloseThis()
    {
        ViewManager.I.CloseView(GetHandle());
    }
}