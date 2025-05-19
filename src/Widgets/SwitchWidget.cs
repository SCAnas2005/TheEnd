



using System;
using System.Reflection.Metadata;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public class SwitchWidget : Widget
{
    public Action<bool> OnClick;
    public bool _value;
    public Color _activeColor;
    public Color _inactiveColor;

    private float _thumbX;
    private float _targetThumbX;
    private float _thumbSpeed = 10f; // vitesse de l’animation
    private float _rightX, _leftX;


    public SwitchWidget(
        Rectangle rect, 
        bool value = false,
        Color? activeColor = null,
        Color? inactiveColor = null,
        bool debug = false,
        Action<bool> OnClick = null
        ) : base(rect)
    {
        _value = value;
        _activeColor = activeColor ?? Color.Green;
        _inactiveColor = inactiveColor ?? Color.Gray;

        _debug = debug;
        this.OnClick = OnClick;
    }


    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
        _leftX = _rect.X;
        _rightX = _rect.X+_rect.Width-(_rect.Height/2)*2;
        _thumbX = _value ? _rightX : _leftX;
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
        _leftX = _rect.X;
        _rightX = _rect.X+_rect.Width-(_rect.Height/2)*2;
        _thumbX = _value ? _rightX : _leftX;
    }}

    public SwitchWidget(
        Size size,  
        bool value = false,
        Color? activeColor = null,
        Color? inactiveColor = null,
        bool debug = false,
        Action<bool> OnClick = null
        ) : this(new Rectangle(0,0,size.Width, size.Height), value, activeColor, inactiveColor, debug, OnClick) 
    {}

    public SwitchWidget(  
        bool value = false,
        Color? activeColor = null,
        Color? inactiveColor = null,
        bool debug = false,
        Action<bool> OnClick = null
        ) : this(Rectangle.Empty, value, activeColor, inactiveColor, debug, OnClick) 
    {}

    public override void Load(ContentManager Content) {
        _leftX = _rect.X;
        _rightX = _rect.X+_rect.Width-(_rect.Height/2)*2;
        _thumbX = _value ? _rightX : _leftX;
    }   

    public void Action()
    {
        if (OnClick != null)
            OnClick(_value);
    }

    public override void Update(GameTime gameTime)
    {
        
        _targetThumbX = _value ? _rightX : _leftX;
        if(InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            _value = !_value;
            Action();
        }
        _thumbX += (_targetThumbX - _thumbX) * 0.1f;
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        Shape.DrawRoundedRectangle(_spriteBatch, _rect, 2, 18, [_value ? _activeColor : _inactiveColor], [_value ? _activeColor : _inactiveColor]);
        Shape.FillCercle(_spriteBatch, new Vector2(_thumbX, _rect.Y), _rect.Height/2, Color.SkyBlue);
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
            Shape.DrawRectangle(_spriteBatch, new Rectangle((int)_leftX, _rect.Y, _rect.Height/2, _rect.Height), Color.Blue);
            Shape.DrawRectangle(_spriteBatch, new Rectangle((int)_rightX, _rect.Y, _rect.Height/2, _rect.Height), Color.Blue);

        }
    }
}