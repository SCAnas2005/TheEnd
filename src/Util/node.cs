using System;
using Microsoft.Xna.Framework;

public class Node
{
    public Point Position;
    public float G, H;
    public float F => G + H;
    public Node Parent;

    public Node(Point position, float g, float h, Node parent = null)
    {
        Position = position;
        G = g;
        H = h;
        Parent = parent;
    }

    public override bool Equals(object obj)
        => obj is Node other && Position == other.Position;

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
