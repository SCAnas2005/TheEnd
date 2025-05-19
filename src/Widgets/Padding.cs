
using Microsoft.Xna.Framework;

public class Padding
{
    private int _left;
    private int _top;
    private int _right;
    private int _bottom;

    public int Left{get{return _left;} set{_left = value; }}
    public int Top{get{return _top;} set{_top = value; }}
    public int Right{get{return _right;} set{_right = value; }}
    public int Bottom{get{return _bottom;} set{_bottom = value; }}


    public Padding(int left = 0, int top = 0, int right = 0, int bottom = 0)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public Padding(int all) : this(left:all, right:all, top:all, bottom:all) {}

    public Rectangle AddToRect(Rectangle rect)
    {
        return new Rectangle(
            x: rect.X+Left,
            y: rect.Y+Top,
            width: rect.Width-Left-Right,
            height: rect.Height-Top-Bottom
        );
    }
}