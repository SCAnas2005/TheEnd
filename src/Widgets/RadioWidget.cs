

using System;
using System.Collections.Generic;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class RadioWidget<T> : Widget
{
    private T _groupValue;
    private T _value;
    private bool _drawable;



    private Action<T> _OnChange;


    public T Value {get{return _value; } set{_value = value;}}
    public T GroupValue {get{return _groupValue; } set{_groupValue = value;}}


    public RadioWidget(Rectangle rect, T value, T groupValue, Action<T> OnChange = null) : base(rect)
    {
        _groupValue = groupValue;
        _value = value;

        _OnChange = OnChange;
    }

    public RadioWidget(Size size, T value, T groupValue, Action<T> OnChange = null) : this(new Rectangle(0,0,size.Width,size.Height), value:value, groupValue:groupValue, OnChange: OnChange) {}

    public override void Load(ContentManager Content)
    {

    }
    public override void Update(GameTime _gameTime) {
        if (InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            if (_OnChange != null)
                _OnChange(_value);
        }
        _drawable = Equals(_groupValue, _value);

    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        // Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        Shape.DrawCircle(_spriteBatch, new Vector2(_rect.X, _rect.Y), _rect.Width/2-1, Color.Blue);
        if (_drawable)
                Shape.FillCercle(_spriteBatch, new Vector2(_rect.X, _rect.Y), _rect.Width/2-1, Color.Blue);

    }

}