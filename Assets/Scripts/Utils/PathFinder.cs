
using System;
using System.Collections.Generic;

/// <summary>
/// 寻路算法
/// </summary>
public static class PathFinder
{
    /// <summary>
    /// 寻路结果节点
    /// </summary>
    public class PathNode
    {
        public int x, y;
        public PathNode preNode;

        public PathNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public PathNode(int x, int y, PathNode preNode)
        {
            this.x = x;
            this.y = y;
            this.preNode = preNode;
        }
    }

    //public List<PathNode> Find(int startX, int startY, int targetX, int targetY, 
    //    Func<bool, int, int> ableMove)
    //{

    //}
}