



using System;
using System.Collections.Generic;
using System.Linq;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


public class DropdownWidget<T> : Widget
{
    private List<T> _values;
    private T _selectedValue;


    private List<ContainerWidget> _items;
    private ContainerWidget _dropDown;
    private IconWidget _icon;
    private ContainerWidget _buttonContainer;
    private TextButtonWidget _button;

    private bool _droped;

    public Action<T> OnChange;

    public DropdownWidget(
        Rectangle rect, 
        List<T> values = null,
        T initialValue = default,
        Action<T> OnChange = null,
        bool debug = false
        ) : base(rect, debug: debug) 
    {
        _droped = false;
        _values = values ?? [];
        _selectedValue = EqualityComparer<T>.Default.Equals(initialValue, default) ? _values.First() : initialValue;
        _items = new List<ContainerWidget>();

        _icon = new IconWidget(size: new Size(_rect.Height/4*3, _rect.Height/4*3), icon: _droped ? Icons.Dropdown : Icons.Dropup);
        _button = new TextButtonWidget(size: new Size(_rect.Width-_rect.Height/4*3, _rect.Height),text: new TextWidget($"{_selectedValue}"));
        _buttonContainer = new ContainerWidget(
            rect: _rect,
            alignItem: Align.Horizontal,
            mainAxisAlignment: MainAxisAlignment.SpaceBetween,
            backgroundColor: new Color(35,35,35),
            padding: new Padding(right: 5),
            widgets: [
                _button,
                _icon
            ]
        );
        _dropDown = new ContainerWidget();
        BuildItemWidgets();

        _debug = debug;
        this.OnChange = OnChange;
    }

    public DropdownWidget(
        Size size,  
        List<T> values = null,
        T initialValue = default,
        Action<T> OnChange = null,
        bool debug = false
        ) : this(new Rectangle(0,0,size.Width, size.Height), values, initialValue, OnChange, debug) 
    {}

    public DropdownWidget(  
        List<T> values = null,
        T initialValue = default,
        bool debug = false,
        Action<T> OnChange = null
        ) : this(Rectangle.Empty, values, initialValue, OnChange, debug) 
    {}

    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
        _buttonContainer.X = _rect.X;
        _dropDown.X = _rect.X;

    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
        _buttonContainer.Y = _rect.Y;
        _dropDown.Y = _rect.Y;
    }}

    public override void Load(ContentManager Content) {
        _buttonContainer.Load(Content);
        _dropDown.Load(Content);
    }   

    public void BuildItemWidgets()
    {
        _items = [];
        int maxWidth = 0;
        int height = 0;
        ContainerWidget c;
        
        foreach (T item in _values)
        {
            TextButtonWidget t = new TextButtonWidget(size: new Size(100,20),
                text: new TextWidget(
                    $"{item}", 
                    color: Color.Black
                ), 
                OnClick:() => {
                        _selectedValue = item;
                        _button.Content = $"{_selectedValue}";
                        BuildItemWidgets();
                        Action();
                        Drop(false);
                },
                backgroundColor: EqualityComparer<T>.Default.Equals(_selectedValue, item) ? Color.Lerp(Color.White, Color.Gray, 0.5f) : Color.White
            );  
            t.OnHover = () => {
                t.BackgroundColor = Color.SkyBlue;
            };
            t.OnNotHover = () => {
                t.BackgroundColor = EqualityComparer<T>.Default.Equals(_selectedValue, item) ? Color.Lerp(Color.White, Color.Gray, 0.5f) : Color.White;
            };
            c = new ContainerWidget(
                alignItem: Align.Horizontal,
                mainAxisAlignment: MainAxisAlignment.Center,
                crossAxisAlignment: CrossAxisAlignment.Center,
                widgets: [t]
            );
            if (c.Size.Width > maxWidth) maxWidth = c.Size.Width;
            height += c.Size.Height;
            _items.Add(c);
        }

        ContainerWidget main = new ContainerWidget(
            rect: new Rectangle(_rect.X, _rect.Y, maxWidth, height),
            alignItem: Align.Vertical,
            widgets: _items.ToArray(),
            debug: true
        );

        _dropDown = main;
    }

    public void Action()
    {
        if (OnChange != null)
            OnChange(_selectedValue);
    }

    public void Drop(bool? val = null)
    {
        _droped = val ?? !_droped;
        _icon.Icon = _droped ? Icons.Dropdown : Icons.Dropup;
    }

    public override void Update(GameTime gameTime)
    {
        if(InputManager.LeftClicked && InputManager.Hover(_rect) && _droped == false)
        {
            Drop();
        } else {
            if (_droped) {
                _dropDown.Update(gameTime);
            } else {
                _buttonContainer.Update(gameTime);
            }
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
        _buttonContainer.Draw(_spriteBatch);
        if (_droped)
        {
            _dropDown.Draw(_spriteBatch);
        }
    }
}