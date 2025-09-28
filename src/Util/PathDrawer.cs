
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class PathDrawer
{
    public static List<List<Vector2>> paths = new();
    public static int AddPath(List<Vector2> path)
    {
        int i = paths.Count;
        paths.Add(path);
        return i;
    }
    public static void RemovePathAt(int index)
    {
        paths.RemoveAt(index);
    }

    public static void Draw(SpriteBatch _spriteBatch)
    {
        foreach (var path in paths)
        {
            if (path != null)
            {
                Vector2 current, old;
                for (int i = 1; i < path.Count; i++)
                {
                    old = path[i - 1];
                    current = path[i];

                    Shape.DrawLine(_spriteBatch, old, current, Color.Yellow);
                    if (i == 1)
                    {
                        Shape.FillRectangle(_spriteBatch, new Rectangle(new Point((int)old.X - 5, (int)old.Y - 5), new Point(10, 10)), Color.Yellow);
                    }
                    else if (i == path.Count - 1)
                    {
                        Shape.FillCercle(_spriteBatch, new Vector2(current.X - 5, current.Y - 5), 5, Color.Yellow);
                    }
                }
            }
        }
    }
}