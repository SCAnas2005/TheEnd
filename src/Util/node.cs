using System;

public class Node
{
    public int X, Y;
    public int G, H;
    public int F => G + H;
    public Node Parent;

    public Node(int x, int y, Node parent = null)
    {
        X = x;
        Y = y;
        Parent = parent;
    }

    public override bool Equals(object obj)
        => obj is Node other && X == other.X && Y == other.Y;

    public override int GetHashCode()
        => HashCode.Combine(X, Y);
}
