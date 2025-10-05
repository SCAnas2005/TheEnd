

using Microsoft.Xna.Framework;

public class MapZone
{
    private Rectangle _rect;
    public Rectangle Rect { get => _rect; }

    private string _name;
    public string Name { get => _name; }

    public MapZone(Rectangle rect, string name)
    {
        _rect = rect;
        _name = name;
    }


    public bool EntityIn(Entity e)
    {
        return Rect.Intersects(e.Rect);
    }
}