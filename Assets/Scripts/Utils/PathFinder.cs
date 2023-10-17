
using JetBrains.Annotations;
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
        public PathNode preNode = null;

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

        public override string ToString()
        {
            return $"PathNode:({x}, {y})";
        }
    }

    /// <summary>
    /// 根据步数得出可行的所有路径
    /// </summary>
    /// <param name="startX">起始x坐标</param>
    /// <param name="startY">起始y坐标</param>
    /// <param name="steps">最大步数</param>
    /// <param name="targetX">目的x坐标</param>
    /// <param name="targetY">目的y坐标</param>
    /// <param name="ableMove">是否可以移动</param>
    /// <returns>路径反向链表的所有节点</returns>
    public static List<PathNode> Find(int startX, int startY, int steps,
        Func<int, int, bool> ableMove)
    {
        PathNode start = new PathNode(startX, startY);
        Queue<PathNode> queue = new Queue<PathNode>();
        ValueTuple<int, int>[] dp =
        {
            new ValueTuple<int, int>(1, 0),
            new ValueTuple<int, int>(0, -1),
            new ValueTuple<int, int>(-1, 0),
            new ValueTuple<int, int>(0, 1),
        };
        queue.Enqueue(start);
        List<PathNode> result = new List<PathNode>();
        HashSet<ValueTuple<int, int>> visited = new HashSet<(int, int)>
        {
            (startX, startY)
        };

        for (int i = 0; i < steps; i++)
        {
            int curLen = queue.Count;
            for (int k = 0; k < curLen; k++)
            {
                PathNode node = queue.Dequeue();
                //result.Add(node);
                for (int j = 0; j < 4; j++)
                {
                    int nx = node.x + dp[j].Item1;
                    int ny = node.y + dp[j].Item2;
                    if (!visited.Contains((nx, ny)) && ableMove(nx, ny))
                    {
                        var newNode = new PathNode(nx, ny, node);
                        visited.Add((nx, ny));
                        queue.Enqueue(newNode);
                        result.Add(newNode);
                    }
                }
            }
        }

        return result;
    }
}