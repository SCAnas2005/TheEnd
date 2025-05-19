



using System;
using System.Reflection.Metadata;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public class CheckBoxWidget : Widget
{
    private Texture2D _bgUnChecked = null;
    private Texture2D _bgChecked = null;
    private Texture2D _bg = null;

    TextWidget _textWidget = null;

    private string _bgUnCheckedAsset;
    private string _bgCheckedAsset;
    
    private Color _forgroundColor;

    private bool _state = false;


    public bool _isClicked = false;

    public Action<bool> OnClick;


    public CheckBoxWidget(
        Rectangle rect, 
        string bgCheckedAsset = null, 
        string bgUnCheckedAsset = null, 
        Color? forgroundColor = null,
        bool state = false,
        bool debug = false,
        Action<bool> OnClick = null
        ) : base(rect)
    {
        _bgUnCheckedAsset = bgUnCheckedAsset ?? IconsManager.GetIconPath(Icons.CheckboxUnChecked);
        _bgCheckedAsset = bgCheckedAsset ?? IconsManager.GetIconPath(Icons.CheckboxChecked);
        _state = state;
        _debug = debug;
        _forgroundColor = forgroundColor ?? Color.White;
        Console.WriteLine($"color {forgroundColor}");


    }

    public CheckBoxWidget(
        Size size, 
        string bgCheckedAsset = null, 
        string bgUnCheckedAsset = null, 
        Color? forgroundColor = null,
        bool state = false,
        bool debug = false,
        Action<bool> OnClick = null
        ) : this(new Rectangle(0,0,size.Width, size.Height), bgCheckedAsset, bgUnCheckedAsset, forgroundColor, state, debug, OnClick) 
    {}

    public CheckBoxWidget( 
        string bgCheckedAsset = null, 
        string bgUnCheckedAsset = null, 
        Color? forgroundColor = null,
        bool state = false,
        bool debug = false,
        Action<bool> OnClick = null
        ) : this(Rectangle.Empty, bgCheckedAsset, bgUnCheckedAsset, forgroundColor, state, debug, OnClick) 
    {}

    public override void Load(ContentManager Content) {
        _bgUnChecked = Content.Load<Texture2D>(_bgUnCheckedAsset);
        _bgChecked = Content.Load<Texture2D>(_bgCheckedAsset);

        _bg = _state ? _bgChecked : _bgUnChecked;
    }   

    public bool IsClicked() { return _isClicked; }

    public void Action()
    {
        if (OnClick != null)
            OnClick(_state);
    }

    public override void Update(GameTime gameTime)
    {
        if(InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            _state = !_state;
            _bg = _state ? _bgChecked : _bgUnChecked;
            Action();
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw(_bg, _rect, _forgroundColor);
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
    }
}