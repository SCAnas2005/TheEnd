
using System;
using System.Security.Cryptography.X509Certificates;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public abstract class Widget 
{
    protected Rectangle _rect;
    protected bool _debug;

    public Action OnClick;
    public Action OnHover;
    public Action OnNotHover;

    public Widget (Rectangle rect, Action OnClick=null, Action OnHover=null, Action OnNotHover = null, bool debug = false) {
        _rect = rect;
        this.OnClick = OnClick;
        this.OnHover = OnHover;
        this.OnNotHover = OnNotHover;
        _debug = debug;
    }

    public Rectangle GetRect() { return _rect; }

    public abstract void Load(ContentManager Content);
    public virtual void Update(GameTime gameTime) {
        if (InputManager.Hover(_rect)) OnHover?.Invoke();
        else OnNotHover?.Invoke();

        if (InputManager.LeftClicked && InputManager.Hover(_rect)) OnClick?.Invoke();
    }
    public virtual void Draw(SpriteBatch _spriteBatch) {
        if (_debug){
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
    }


    public virtual Vector2 Position {get {return new Vector2(_rect.X, _rect.Y); } set { _rect.X = (int)value.X; _rect.Y = (int)value.Y; }}
    public virtual Size Size {get {return new Size(_rect.Width, _rect.Height);} set {_rect.Width = value.Width; _rect.Height = value.Height;}}
    public  virtual Rectangle Rect {
        get {return _rect;}
        set {_rect = value; }
    }

    public virtual int X {get {return _rect.X;} set {_rect.X = value;}}
    public virtual int Y {get {return _rect.Y;} set {_rect.Y = value;}}

}