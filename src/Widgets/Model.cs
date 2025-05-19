using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


public class Example : Widget
{
    public Example(
        Rectangle rect, 
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick:OnClick, debug: debug)
    {
        _debug = debug;
        this.OnClick = OnClick;
    }

    public Example(
        Size size, 
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0,0,size.Width, size.Height), debug, OnClick) 
    {}

    public Example( 
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, debug, OnClick) 
    {}

    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
    }}

    public override void Load(ContentManager Content) {
        
    }   

    public void Action()
    {
        if (OnClick != null)
            OnClick();
    }

    public override void Update(GameTime gameTime)
    {
        if(InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            Action();
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
    }
}