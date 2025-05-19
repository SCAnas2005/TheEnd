using System;
using System.Reflection.Metadata;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public class Border
{
    private int _left,_right,_top,_bottom;
    private Color _color;
    
    public Color Color {get{return _color;} set{_color = value;}}
    public int Left {get{return _left;} set{_left = value;}}
    public int Right {get{return _right;} set{_right = value;}}
    public int Top {get{return _top;} set{_top= value;}}
    public int Bottom {get{return _bottom;} set{_bottom = value;}}

    public Border(int left=0, int top=0, int right=0, int bottom=0, Color? color = null)
    {
        Left = left; Top = top; Right = right; Bottom = bottom;
        Color = color ?? Color.Black;
    }

    public Border(int all = 0, Color? color = null) : this(left:all, top:all, right:all, bottom:all, color:color) {}

    public Border(int horizontal=0, int vertical=0, Color? color = null) : this(left:vertical, top:horizontal, right:vertical, bottom:horizontal, color:color) {}


    public void Draw(SpriteBatch _spriteBatch, Rectangle rect, int dx=0, int dy=0)
    {
        if (Left == 0 && Right == 0 && Top == 0 && Bottom == 0) return;
        Vector2 start, end;
        if (Left > 0)
        {
            start = new Vector2(rect.X, rect.Y+dy); end = new Vector2(rect.X, rect.Y+rect.Height-dy*2);
            Shape.DrawLine(_spriteBatch, start, end, Color, Left);
        }
        if (Top > 0)
        {
            start = new Vector2(rect.X+dx, rect.Y); end = new Vector2(rect.X+rect.Width-dx*2, rect.Y);
            Shape.DrawLine(_spriteBatch, start, end, Color, Top);
        }
        if (Right > 0)
        {
            start = new Vector2(rect.X+rect.Width, rect.Y+dy); end = new Vector2(rect.X+rect.Width, rect.Y+rect.Height-dy*2);
            Shape.DrawLine(_spriteBatch, start, end, Color, Right);
        }
        if (Bottom > 0)
        {
            start = new Vector2(rect.X+dx, rect.Y+rect.Height); end = new Vector2(rect.X+rect.Width-dx*2, rect.Y+rect.Height);
            Shape.DrawLine(_spriteBatch, start, end, Color, Bottom);
        }
    }

}