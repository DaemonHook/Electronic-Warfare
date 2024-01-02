using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
static class Util
{
    /// <summary>
    /// 挂在协程上，实现延迟一定帧数后执行
    /// </summary>
    public static IEnumerator DelayCorotine(int frames, Action action)
    {
        for (int i = 0; i < frames; i++)
        {
            // Debug.Log($"frame: {Time.frameCount}");
            yield return null;
        }
        action();
    }
}
