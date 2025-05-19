

using System;
using System.Reflection.Metadata;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public enum Button{
    Play,
    Continue,
    NewGame,
    Settings,
    Audio,
    Music,
    Home,
    Return,
    Controls,
    Exit,
    OnOff,
    Quit,

    Empty,
}

public class ButtonWidget : Widget
{
    protected Texture2D _bg = null;
    protected Texture2D _bgHover = null;
    protected Texture2D _mainBg = null;


    protected Color? _bgColor;

    protected TextWidget _textWidget = null;


    protected bool _hover = false;

    protected string _bgAsset;
    protected string _bgHoverAsset;


    public Color? BackgroundColor {get{return _bgColor;} set{_bgColor = value;}}

    public bool _isClicked = false;

    public ButtonWidget(
        Rectangle rect, 
        string backgroundAsset, 
        TextWidget text = null,
        bool hover=false, 
        string backgroundHoverAsset = null, 
        Color? backgroundColor = null,
        Action OnClick = null,
        Action OnHover = null,
        Action OnNotHover = null,
        bool debug = false
        ) : base(rect, OnClick: OnClick, OnHover:OnHover, OnNotHover: OnNotHover, debug)
    {
        _bgAsset = backgroundAsset;
        _hover = hover;
        if (_hover && _bgAsset != null)
            _bgHoverAsset = _bgAsset+"_hover";

        _textWidget = text;
        if (_textWidget != null)
        {
            _textWidget.Position = new Vector2(_rect.X+(_rect.Width-_textWidget.Size.Width)/2, _rect.Y+(_rect.Height-_textWidget.Size.Height)/2);
            if (_rect.IsEmpty)
            {

            }
        }

        _bgColor = backgroundColor;

        _debug = debug;
    }

    public ButtonWidget(
        Size size, 
        string backgroundAsset, 
        TextWidget text = null,
        bool hover=false, 
        string backgroundHoverAsset = null, 
        Color? backgroundColor = null,
        Action OnClick = null,
        Action OnHover = null,
        Action OnNotHover = null,
        bool debug = false
        ) : this(new Rectangle(0,0,size.Width, size.Height), backgroundAsset, text, hover, backgroundHoverAsset, backgroundColor, OnClick, OnHover, OnNotHover, debug) {}
    

    public ButtonWidget(
        Size size, 
        Texture2D background, 
        TextWidget text = null,
        bool hover=false, 
        string backgroundHoverAsset = null, 
        Color? backgroundColor = null,
        Action OnClick = null,
        Action OnHover = null,
        Action OnNotHover = null,
        bool debug = false
    ) : base(new Rectangle(0,0,size.Width,size.Height), OnClick: OnClick, OnHover:OnHover, OnNotHover: OnNotHover, debug) {
        _bg = background;
        _hover = hover;
        if (_hover)
            _bgHoverAsset = _bgAsset+"_hover";
     ;

        _textWidget = text;
        if (_textWidget != null)
            _textWidget.Position = new Vector2(_rect.X+(_rect.Width-_textWidget.Size.Width)/2, _rect.Y+(_rect.Height-_textWidget.Size.Height)/2);

        _bgColor = backgroundColor;
        _debug = debug;
    }


    public ButtonWidget( 
        string backgroundAsset = null, 
        TextWidget text = null,
        bool hover=false, 
        string backgroundHoverAsset = null, 
        Color? backgroundColor = null,
        Action OnClick = null,
        Action OnHover = null,
        Action OnNotHover = null,
        bool debug = false
    ) : this(Rectangle.Empty, backgroundAsset, text, hover, backgroundHoverAsset, backgroundColor, OnClick, OnHover, OnNotHover, debug) 
    {
            
    }


    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
        if (_textWidget != null)
        {
            _textWidget.Position = new Vector2(_rect.X+(_rect.Width-_textWidget.Size.Width)/2, _rect.Y+(_rect.Height-_textWidget.Size.Height)/2);
        }
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
        if (_textWidget != null)
        {
            _textWidget.Position = new Vector2(_rect.X+(_rect.Width-_textWidget.Size.Width)/2, _rect.Y+(_rect.Height-_textWidget.Size.Height)/2);
        }
    }}

    public override Rectangle Rect {get {return _rect;} set {
        _rect = value; 
        if (_textWidget != null)
        {
            _textWidget.Position = new Vector2(_rect.X+(_rect.Width-_textWidget.Size.Width)/2, _rect.Y+(_rect.Height-_textWidget.Size.Height)/2);
        }
    }}

    public override void Load(ContentManager Content) {
        if (_bg == null && _bgAsset != null)
        {
            _bg = Content.Load<Texture2D>(_bgAsset);

        }
        _mainBg = _bg;
        if (_hover && _bgHoverAsset != null)
            _bgHover = Content.Load<Texture2D>(_bgHoverAsset);
        else 
            _bgHover = _bg;
        
        if (_textWidget != null)
        {
            _textWidget.Load(Content);
        }

        if (Size.IsEmpty)
        {
            if (_bg != null)
            {
                Size = new Size(_bg.Width, _bg.Height);
            } else if (_textWidget != null) {
                Size = new Size(_textWidget.Size.Width+20, _textWidget.Size.Height+20);
            }
        }
    }   

    public bool IsClicked() { return _isClicked; }

    public void Action()
    {
        OnClick();
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Hover(_rect)) {
            OnHover?.Invoke();
        } else {
            OnNotHover?.Invoke();
        }
        if (_hover)
        {   
            _mainBg = InputManager.Hover(_rect) ? _bgHover : _bg;
        }
        _isClicked = InputManager.LeftClicked && InputManager.Hover(_rect);

        if (_textWidget != null) _textWidget.Update(gameTime);

        if (_isClicked && OnClick != null) 
        {
            
            Action();
        }

    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (_bgColor != null)
        {
            Shape.FillRectangle(_spriteBatch, _rect, (Color)_bgColor);
        }
        if (_mainBg != null)
            _spriteBatch.Draw(_mainBg, _rect, Color.White);
        
        if (_textWidget!=null)
        {
            _textWidget.Draw(_spriteBatch);
        }

        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
    }
}