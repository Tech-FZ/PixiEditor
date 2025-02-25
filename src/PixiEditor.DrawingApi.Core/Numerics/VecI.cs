﻿using System;
using PixiEditor.DrawingApi.Core.Surface;

namespace PixiEditor.DrawingApi.Core.Numerics;

public struct VecI : IEquatable<VecI>
{
    public int X { set; get; }
    public int Y { set; get; }

    public int TaxicabLength => Math.Abs(X) + Math.Abs(Y);
    public double Length => Math.Sqrt(LengthSquared);
    public int LengthSquared => X * X + Y * Y;
    public int LongestAxis => (Math.Abs(X) < Math.Abs(Y)) ? Y : X;
    public int ShortestAxis => (Math.Abs(X) < Math.Abs(Y)) ? X : Y;

    public static VecI Zero { get; } = new(0, 0);
    public static VecI One { get; } = new(1, 1);

    public VecI(int x, int y)
    {
        X = x;
        Y = y;
    }
    public VecI(int bothAxesValue)
    {
        X = bothAxesValue;
        Y = bothAxesValue;
    }
    public VecI Signs()
    {
        return new VecI(X >= 0 ? 1 : -1, Y >= 0 ? 1 : -1);
    }
    public VecI Multiply(VecI other)
    {
        return new VecI(X * other.X, Y * other.Y);
    }
    public VecI Add(int value)
    {
        return new VecI(X + value, Y + value);
    }
    /// <summary>
    /// Reflects the vector across a vertical line with the specified x position
    /// </summary>
    public VecI ReflectX(int lineX)
    {
        return new(2 * lineX - X, Y);
    }
    /// <summary>
    /// Reflects the vector across a horizontal line with the specified y position
    /// </summary>
    public VecI ReflectY(int lineY)
    {
        return new(X, 2 * lineY - Y);
    }
    public static VecI operator +(VecI a, VecI b)
    {
        return new VecI(a.X + b.X, a.Y + b.Y);
    }
    public static VecI operator -(VecI a, VecI b)
    {
        return new VecI(a.X - b.X, a.Y - b.Y);
    }
    public static VecI operator -(VecI a)
    {
        return new VecI(-a.X, -a.Y);
    }
    public static VecI operator *(int b, VecI a)
    {
        return new VecI(a.X * b, a.Y * b);
    }
    public static int operator *(VecI a, VecI b)
    {
        return a.X * b.X + a.Y * b.Y;
    }
    public static VecI operator *(VecI a, int b)
    {
        return new VecI(a.X * b, a.Y * b);
    }
    public static VecD operator *(VecI a, double b)
    {
        return new VecD(a.X * b, a.Y * b);
    }
    public static VecI operator /(VecI a, int b)
    {
        return new VecI(a.X / b, a.Y / b);
    }
    public static VecD operator /(VecI a, double b)
    {
        return new VecD(a.X / b, a.Y / b);
    }
    public static VecI operator %(VecI a, int b)
    {
        return new(a.X % b, a.Y % b);
    }
    public static VecD operator %(VecI a, double b)
    {
        return new(a.X % b, a.Y % b);
    }
    public static bool operator ==(VecI a, VecI b)
    {
        return a.X == b.X && a.Y == b.Y;
    }
    public static bool operator !=(VecI a, VecI b)
    {
        return !(a.X == b.X && a.Y == b.Y);
    }
    public static implicit operator VecI(Point point)
    {
        return new VecI((int)point.X, (int)point.Y);
    }
    
    public static implicit operator Point(VecI vec)
    {
        return new(vec.X, vec.Y);
    }
    
    public static implicit operator VecD(VecI vec)
    {
        return new VecD(vec.X, vec.Y);
    }
    public static implicit operator VecI((int, int) tuple)
    {
        return new VecI(tuple.Item1, tuple.Item2);
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public override string ToString()
    {
        return $"({X}; {Y})";
    }

    public override bool Equals(object? obj)
    {
        var item = obj as VecI?;
        if (item is null)
            return false;
        return this == item;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public bool Equals(VecI other)
    {
        return other.X == X && other.Y == Y;
    }

    public VecD Normalized()
    {
        return this / Length;
    }
}
