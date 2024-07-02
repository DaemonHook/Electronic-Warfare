using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;

public class Coordinate : IEquatable<Coordinate>, IComparable<Coordinate>
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override bool Equals(object obj)
    {
        if (obj is Coordinate)
        {
            return Equals((Coordinate)obj);
        }
        return false;
    }

    public bool Equals(Coordinate other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        // 不检查溢出
        unchecked
        {
            // 33哈希
            int hash = 17;
            hash = hash * 33 + X.GetHashCode();
            hash = hash * 33 + Y.GetHashCode();
            return hash;
        }
    }

    public int CompareTo(Coordinate other)
    {
        int compareX = X.CompareTo(other.X);
        if (compareX == 0)
        {
            return Y.CompareTo(other.Y);
        }
        return compareX;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public static bool operator ==(Coordinate left, Coordinate right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Coordinate left, Coordinate right)
    {
        return !(left == right);
    }

    public static bool operator <(Coordinate left, Coordinate right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(Coordinate left, Coordinate right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(Coordinate left, Coordinate right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(Coordinate left, Coordinate right)
    {
        return left.CompareTo(right) >= 0;
    }

    public Coordinate GetNeighbor(Direction dir)
    {
        return dir switch
        {
            Direction.LEFT => new Coordinate(X - 1, Y),
            Direction.RIGHT => new Coordinate(X + 1, Y),
            Direction.DOWN => new Coordinate(X, Y - 1),
            Direction.UP => new Coordinate(X, Y + 1),
            _ => throw new ArgumentException("非法的方向"),
        };
    }
}