using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// 检索特定范围内符合条件的所有点
/// </summary>
public class Scanner
{
    bool IsWithinRange(Vector2Int point, Vector2Int start, Vector2Int end)
    {
        return point.x >= Mathf.Min(start.x, end.x)
            && point.x <= Mathf.Max(start.x, end.x)
            && point.y >= Mathf.Min(start.y, end.y)
            && point.y <= Mathf.Max(start.y, end.y);
    }

    private Vector2Int start;
    private Vector2Int border;
    private int length;
    private Func<Vector2Int, bool> legal;

    public Scanner(Vector2Int start, Vector2Int border, int length, Func<Vector2Int, bool> isLegal)
    {
        this.start = start;
        this.border = border;
        this.length = length;
        this.legal = isLegal;
    }

    private static Vector2Int[] dp =
        {
            new (1, 0),
            new (0, -1),
            new (-1, 0),
            new (0, 1),
        };

    public List<Vector2Int> Scan()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        queue.Enqueue(start);
        visited.Add(start);
        for (int i = 0; i < length && queue.Count > 0; i++)
        {
            int c = queue.Count;
            for (int j = 0; j < c; j++)
            {
                Vector2Int p = queue.Dequeue();
                if (legal(p))
                {
                    list.Add(p);
                }
                foreach (var d in dp)
                {
                    var newP = p + d;
                    if (!visited.Contains(newP) &&
                        IsWithinRange(newP, new(0, 0), border))
                    {
                        visited.Add(newP);
                        queue.Enqueue(newP);
                    }
                }
            }
        }

        return list;
    }
}
