
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheEnd;
using System.Diagnostics;
using System;

public class TextWidget : Widget
{
    private string _text = null;
    private string _wrappedText = null;
    private SpriteFont _font;
    private int _fontSize;
    private Color _color;
    private Color? _backgroundColor;
    private Rectangle _textRect;

    public override int X {get {return _rect.X;} set {_rect.X = value; _textRect.X = _rect.X; Wrap(); UpdateSize();}}
    public override int Y {get {return _rect.Y;} set {_rect.Y = value; _textRect.Y = _rect.Y; Wrap(); UpdateSize();}}
    public override Vector2 Position { get => new Vector2(_rect.X, _rect.Y); set {_rect.X = (int)value.X; _rect.Y = (int)value.Y; _textRect.X = (int)value.X; _textRect.Y = (int)value.Y; Wrap(); UpdateSize();} }
    public override Rectangle Rect {get {return _rect;} set {_rect = value; _textRect = new Rectangle(_rect.X, _rect.Y, _textRect.Width, _textRect.Height); Wrap(); UpdateSize();}}

    public TextWidget(string text,
        Rectangle rect,
        SpriteFont font = null,
        int? fontSize = null,
        Color? color = null,
        Color? backgroundColor = null,
        bool debug = false
    ) : base(rect, debug:debug)
    {
        _text = text;
        _wrappedText = text;
        _font = font ?? CFonts.DefaultFont;
        _fontSize = fontSize ?? 12;
        _color = color ?? Color.White;
        _backgroundColor = backgroundColor;

        Size s = Text.GetSize(_text, _font);
        Content = _text;
        Size = s;
        _textRect = new Rectangle(Rect.X, Rect.Y, Size.Width, Size.Height);
        Wrap();
    }
    public TextWidget(
        string text,
        Vector2 position,
        SpriteFont font = null,
        int? fontSize = null,
        Color? color = null,
        Color? backgroundColor = null,
        bool debug = false
    ) : this(text, new Rectangle((int)position.X, (int)position.Y, 0, 0), font:font, color:color, backgroundColor:backgroundColor, debug:debug) 
    {}

    public TextWidget(
        string text,
        SpriteFont font = null,
        int? fontSize = null,
        Color? color = null,
        Color? backgroundColor = null,
        bool debug = false
    ) : this(text, Rectangle.Empty, font:font, color:color, backgroundColor:backgroundColor, debug:debug) 
    {}

    public string Content {get {return _text;} set{_text = value; Wrap(); UpdateSize();} }

    public void UpdateSize()
    {
        Size s = Text.GetSize(_wrappedText, _font);
        Size = s;
        _textRect = new Rectangle(Rect.X, Rect.Y, Size.Width, Size.Height);
    }


    public void Wrap()
    {
        _wrappedText = "";
        if (_textRect.Width > _rect.Width)
        {
            string line = "";
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];
                if (c == '\n')
                {
                    _wrappedText += line + "\n";
                    line = "";
                }
                else
                {
                    line += c;
                }
                int n = Text.GetSize(line, _font).Width;
                if (n >= _rect.Width - Text.GetSize(c + "", _font).Width)
                {
                    _wrappedText += line + "\n";
                    line = "";
                }
            }
            _wrappedText += line;
        }
        else
        {
            _wrappedText = _text;
        }
    }

    public override void Load(ContentManager Content) {}
    public override void Update(GameTime gameTime) {
        
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (_backgroundColor != null)
            Shape.FillRectangle(_spriteBatch, _rect, (Color)_backgroundColor);
        Text.Write(_spriteBatch, _wrappedText, new Vector2(_textRect.X, _textRect.Y), _color, _font);

        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _textRect, Color.Red);
        }
    }
}