
using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public class InputWidget : Widget
{
    private StringBuilder _text = new();
    private string _textToShow = "";
    private TextWidget _textWidget;
    private Rectangle _writingRect;
    private CusorInputWidget _cursorWidget;
    private SpriteFont _font;
    private Action<string> _OnChange;
    private bool _focused;
    private int _cursorIndex;

    private int _indexStart;


    private Color _borderColorOnSelect;

    private Color _backgroundColor;
    private Color _textColor;
    private Color _cursorColor;

    public string Content { get => _text.ToString(); set => _text = new StringBuilder(value); }

    public InputWidget(Rectangle rect,
        String text = "",
        SpriteFont font = null,
        Color? textColor = null,
        Color? backgroundColor = null,
        Color? borderColorOnSelect = null,
        Color? cursorColor = null,
        Action<string> OnChange = null
    ) : base(rect, OnClick: null, OnHover: null, OnNotHover: null, debug: false)
    {
        _font = font ?? CFonts.DefaultFont;
        _focused = false;


        _cursorIndex = 0;
        _indexStart = 0;
        // foreach (char c in text)
        // {
        //     AddInput(c);
        // }


        _writingRect = new Rectangle(_rect.X + 10, _rect.Y + 1, _rect.Width - 10, _rect.Height - 1);
        _textColor = textColor ?? Color.Black;
        _textWidget = new TextWidget(text: _text.ToString(), font: _font, color: _textColor, debug: false);
        _cursorColor = cursorColor ?? Color.Black;
        _cursorWidget = new CusorInputWidget(new Rectangle(_textWidget.X, _textWidget.Y, 1, _textWidget.Size.Height), color: _cursorColor);
        Content = text;

        _OnChange = OnChange;

        _borderColorOnSelect = borderColorOnSelect ?? Color.Blue;
        _backgroundColor = backgroundColor ?? Color.White;
    }

    public InputWidget(
        Size size,
        string text = "",
        SpriteFont font = null,
        Color? textColor = null,
        Color? backgroundColor = null,
        Color? borderColorOnSelect = null,
        Color? cursorColor = null,
        Action<string> OnChange = null

    ) : this(new Rectangle(Vector2.Zero.ToPoint(), size.ToPoint()), text, font, textColor, backgroundColor, borderColorOnSelect, cursorColor, OnChange) {

    }

    public void PrintDebugInfo()
    {
        Console.WriteLine($"=== [INPUT] actualchar:{_text.Length}");
        Console.WriteLine($"=== current_index_pos:{_cursorIndex}, current_printed_text:{_textToShow}, full_text:{_text}");
        Console.WriteLine($"=== index_start:{_indexStart}, index_end:{_textToShow.Length-1}");
        Console.WriteLine();
    }

    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
        UpdateChildsPosition();
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
        UpdateChildsPosition();
    }}

    public string GetContent()
    {
        return _textWidget.Content;
    }

    public override void Load(ContentManager Content)
    {
        _textWidget.Load(Content);
        _cursorWidget.Load(Content);

        UpdateChildsPosition();

    }


    public void UpdateChildsPosition()
    {
        _writingRect = new Rectangle(_rect.X+10, _rect.Y+1, _rect.Width-10, _rect.Height-1);
        _textWidget.Rect = new Rectangle(_writingRect.X, _writingRect.Y+(_writingRect.Height-_textWidget.Size.Height)/2, _textWidget.Size.Width, _textWidget.Size.Height);
        _cursorWidget.Rect = new Rectangle(_textWidget.X, _textWidget.Y, 1, _textWidget.Size.Height);
    }

    public void UpdateDisplayedText()
    {
        if (_text.Length == 0)
        {
            _textToShow = "";
            _textWidget.Content = "";
            _cursorWidget.X = _textWidget.X;
            return;
        }

        _indexStart = Math.Clamp(_indexStart, 0, _text.Length - 1);
        //Texte visible
        _textToShow = Text.GetFittingText(_text.ToString(_indexStart, _text.Length - _indexStart), _font, _writingRect);

        if (_cursorIndex < _indexStart) _indexStart = _cursorIndex;

        while (_cursorIndex > _indexStart + _textToShow.Length) _indexStart++;

        // Calcul largeur jusqu'au curseur
        int relativeIndex = Math.Clamp(_cursorIndex - _indexStart, 0, _textToShow.Length);
        string sub = _textToShow.Substring(0, relativeIndex);
        Size ts = Text.GetSize(sub, _font);

        // Placement curseur
        _cursorWidget.Position = new Vector2(_textWidget.X + ts.Width, _textWidget.Y);

        // Update affichage
        _textWidget.Content = _textToShow;
    }



    public override void Update(GameTime gameTime)
    {
        UpdateChildsPosition();
        UpdateDisplayedText();
        if (InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            _focused = true;
        }
        else if (InputManager.LeftClicked && _focused && !InputManager.Hover(_rect))
        {
            Console.WriteLine("Un focused");
            _focused = false;
        }

        if (_focused)
        {
            FindIndexFromMouse();
            char? key = InputManager.GetPressedKeyRepeat(gameTime);
            AddInput(key);
            // if (_text.Length > 0)
            // {
            //     _textToShow = Text.GetFittingText(_text.ToString(_indexStart, _text.Length - _indexStart), _font, _writingRect);

            //     Size ts = Text.GetSize(_text.ToString(_indexStart, _cursorIndex - _indexStart), _font);
            //     if (ts.Width <= _writingRect.Width)
            //     {
            //         _cursorWidget.X = _textWidget.X + ts.Width;
            //         _cursorWidget.Y = _textWidget.Y;

            //         _textWidget.Content = _textToShow;
            //     }

            // }

            _cursorWidget.Update(gameTime);

        }

        // _container.UpdateLayout();


        _textWidget.Update(gameTime);
    }



    public void AddInput(char? key)
    {
        if (key == null) return;
        if (key == '\b')
        {
            if (_cursorIndex > 0)
            {
                _text.Remove(_cursorIndex - 1, 1);
                _cursorIndex--;
                // if (_textToShow.Length < _text.Length && _cursorIndex - _indexStart >= _textToShow.Length && _text.Length != 0)
                // {
                //     if (_indexStart > 0)
                //         _indexStart--;
                //     else
                //         _indexStart = 0;
                // }

                _OnChange?.Invoke(_text.ToString());

            }
        }
        else if (key == '\u2190') // Left arrow
        {
            if (_cursorIndex > 0)
            {
                _cursorIndex--;
                // if (_cursorIndex <= _indexStart)
                // {
                //     if (_indexStart > 0)
                //         _indexStart--;
                //     else
                //         _indexStart = 0;
                // }
            }
        }
        else if (key == '\u2192') // Right arrow
        {
            if (_cursorIndex < _text.Length)
            {
                _cursorIndex++;
                // if (_cursorIndex - _indexStart > _textToShow.Length && _textToShow.Length < _text.Length)
                // {
                //     _indexStart++;
                // }
            }
        }
        else if (char.IsLetterOrDigit((char)key) || char.IsWhiteSpace((char)key))
        {

            if (_textToShow.Length < _text.Length && _cursorIndex - _indexStart > _textToShow.Length && _text.Length != 0)
            {
                _indexStart++;
            }

            _text.Insert(_cursorIndex, key.ToString());
            _cursorIndex++;
            //_cursorIndex > _indexStart+_maxCharToShow || 
            Console.WriteLine($"[INPUT] Key='{key}' code={(int)key} text='{_text}'");

            _OnChange?.Invoke(_text.ToString());
        }
        PrintDebugInfo();
    }

    public void FindIndexFromMouse()
    {

        if(InputManager.LeftClicked && InputManager.Hover(_writingRect))
        {
            int mouseX = (int)InputManager.GetMousePosition().X;
            int startX = _writingRect.X;

            _cursorIndex = _indexStart;

            for (int i = 0; i < _textToShow.Length; i++)
            {
                string sub = _textToShow.Substring(0, i+1);
                int Width = Text.GetSize(sub, _font).Width;

                if (mouseX < startX + Width)
                {
                    _cursorIndex = _indexStart+i;
                    // Console.WriteLine($"New position for cursor index: {_cursorIndex}");

                    return;
                }
            }      

            _cursorIndex = _indexStart + _text.Length;
            // Console.WriteLine($"New position for cursor index: {_cursorIndex}");


        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        Shape.DrawRoundedRectangle(_spriteBatch, _rect, _focused ? 3 : 2, 5, [_backgroundColor], [_focused ? _borderColorOnSelect : Color.Black]);
        _textWidget.Draw(_spriteBatch);
        // Shape.DrawRectangle(_spriteBatch, _writingRect, Color.Yellow);
        if (_focused)
            _cursorWidget.Draw(_spriteBatch);
    }

}