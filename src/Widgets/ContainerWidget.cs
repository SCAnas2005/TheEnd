

using System;
using System.Collections.Generic;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public enum Align 
{
    Horizontal,
    Vertical,
}

public enum MainAxisAlignment{
    Start,
    Center,
    End,
    SpaceBetween,
    SpaceEvenly,
    SpaceAround,
}

public enum CrossAxisAlignment{
    Start,
    Center,
    End,
}

public class ContainerWidget : Widget
{
    public List<Widget> _widgets;
    private Color? _bgColor;
    private Align _alignItem;
    private MainAxisAlignment _mainAxisAlignment;
    private CrossAxisAlignment _crossAxisAlignment;
    private Padding _padding;
    private Border _border;
    private bool _wrap = false;
    private ImageWidget _backgroundImage;


    public ContainerWidget(
        Rectangle rect,  
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Center,
        Align alignItem = Align.Horizontal, 
        Color? backgroundColor = null,
        ImageWidget backgroundImage = null,
        Padding padding = null,
        Border border = null,
        bool wrap = false,
        Widget[] widgets = null,
        bool debug = false
    ) : base(rect, debug: debug) {
        _widgets = widgets != null ? [.. widgets] : [];
        _alignItem = alignItem;
        _bgColor = backgroundColor;
        _mainAxisAlignment = mainAxisAlignment;
        _crossAxisAlignment = crossAxisAlignment;
        _wrap = wrap;
        _padding = padding ?? new Padding(0); // default = 0 padding
        _border = border ?? new Border(all:0);

        _backgroundImage = backgroundImage ?? null;


        // Work for animation movement and resizement
        // _childsRatio = [];
        // foreach (Widget widget in _widgets)
        // {
        //     _childsRatio.Add((widget.Size.Width != 0 ? Size.Width/widget.Size.Width : 0, widget.Size.Height != 0 ? Size.Height/widget.Size.Height : 0 ));
        // }
        UpdateLayout();
    }

    public ContainerWidget(
        Size size, 
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Center,
        Align alignItem = Align.Horizontal, 
        Color? backgroundColor = null,
        ImageWidget backgroundImage = null,
        Padding padding = null,
        Border border = null,
        bool wrap = false,
        Widget[] widgets = null,
        bool debug = false
    ) : this(new Rectangle(0,0,size.Width, size.Height), mainAxisAlignment, crossAxisAlignment, alignItem, backgroundColor, backgroundImage, padding, border, wrap, widgets, debug) 
    {}


    public override int X {get {return _rect.X;} set {_rect.X = value; UpdateLayout();}}
    public override int Y {get {return _rect.Y;} set {_rect.Y = value; UpdateLayout();}}
    public override Size Size {get {return new Size(_rect.Width, _rect.Height);} 
    set {
        Rect = new Rectangle(Rect.X, Rect.Y, value.Width, value.Height);
    }}
    public override Rectangle Rect {
        get => _rect;
        set{
            _rect = value; 
            //LayoutSize();
            UpdateLayout();
        }
    }

    public ContainerWidget( 
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Center,
        Align alignItem = Align.Horizontal, 
        Color? backgroundColor = null,
        ImageWidget backgroundImage = null,
        Padding padding = null,
        Border border = null,
        bool wrap = false,
        Widget[] widgets = null,
        bool debug = false
    ) : this(Rectangle.Empty, mainAxisAlignment, crossAxisAlignment, alignItem, backgroundColor, backgroundImage, padding, border, wrap, widgets, debug) 
    {   
        Size s = GetChildsSize(_alignItem);
        _rect.Width = s.Width;
        _rect.Height = s.Height;
        UpdateLayout();
    }

    public ContainerWidget(
        Vector2 position, 
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Center,
        Align alignItem = Align.Horizontal, 
        Color? backgroundColor = null,
        ImageWidget backgroundImage = null,
        Padding padding = null,
        Border border = null,
        bool wrap = false,
        Widget[] widgets = null,
        bool debug = false
    ) : this(mainAxisAlignment, crossAxisAlignment, alignItem, backgroundColor, backgroundImage, padding, border, wrap, widgets, debug) 
    {   
        X = (int) position.X; Y = (int) position.Y;
        UpdateLayout();
    }

    public void UpdateLayout(int? startX = null, int? startY = null)
    {
        if (_widgets.Count == 0) return;
        int start_pos = _rect.X;
        int spaceBetween = 0;
        Size childsSize = GetChildsSize(_alignItem);
        int len = _alignItem == Align.Horizontal ? childsSize.Width : childsSize.Height;
        int maxOtherSide = _alignItem == Align.Horizontal ? childsSize.Height : childsSize.Width;

        if (_backgroundImage != null)
            _backgroundImage.Rect = Rect;

        Rectangle layoutRect = _padding.AddToRect(_rect);

        int start_x = startX ?? layoutRect.X, start_y = startY ?? layoutRect.Y;


        switch (_mainAxisAlignment)
        {
            case MainAxisAlignment.Start:
                start_pos = _alignItem == Align.Horizontal ? start_x : start_y;
            break;
            
            case MainAxisAlignment.Center:
                start_pos = _alignItem == Align.Horizontal ? start_x+(layoutRect.Width-len)/2 : start_y+(layoutRect.Height-len)/2;
            break; 

            case MainAxisAlignment.End:
                start_pos = _alignItem == Align.Horizontal ? start_x+layoutRect.Width-len : start_y+layoutRect.Height-len;
            break; 

            case MainAxisAlignment.SpaceBetween:
                if (_widgets.Count > 1)
                    spaceBetween = (_alignItem == Align.Horizontal ? layoutRect.Width-len : layoutRect.Height-len)/(_widgets.Count-1);
                else{
                    spaceBetween = _alignItem == Align.Horizontal ? layoutRect.Width-len : layoutRect.Height-len;
                }
                start_pos = _alignItem == Align.Horizontal ? start_x : start_y;
            break; 

            case MainAxisAlignment.SpaceAround:
                spaceBetween = ((_alignItem == Align.Horizontal ? layoutRect.Width : layoutRect.Height) -len)/(_widgets.Count+1);
                start_pos = _alignItem == Align.Horizontal ? start_x+spaceBetween : start_y+spaceBetween;
            break; 
        }

        int x = _alignItem == Align.Horizontal ? start_pos : start_x;
        int y = _alignItem == Align.Vertical ? start_pos : start_y;

        int temp_x = x, temp_y = y;

        foreach (Widget widget in _widgets)
        {
            widget.X = temp_x;
            widget.Y = temp_y;

            if (_alignItem == Align.Horizontal){
                temp_x+=widget.Rect.Width+spaceBetween;
                if (_crossAxisAlignment == CrossAxisAlignment.Start)
                {
                    temp_y = y;
                } else if (_crossAxisAlignment == CrossAxisAlignment.Center) {
                    temp_y=y+(layoutRect.Height-widget.Rect.Height)/2;
                } else if (_crossAxisAlignment == CrossAxisAlignment.End) {
                    temp_y=y+layoutRect.Height-widget.Rect.Height;
                }
                if (temp_x+widget.Size.Width>layoutRect.X+layoutRect.Width && _wrap)
                {
                    temp_x = x;
                    y+=maxOtherSide;
                }
                widget.Y = temp_y;
            }
            else {    
                temp_y+=widget.Rect.Height+spaceBetween;
                if (_crossAxisAlignment == CrossAxisAlignment.Start)
                {
                    temp_x = x;
                } else if (_crossAxisAlignment == CrossAxisAlignment.Center) {
                    temp_x=x+(layoutRect.Width-widget.Rect.Width)/2;
                } else if (_crossAxisAlignment == CrossAxisAlignment.End) {
                    temp_x=x+layoutRect.Width-widget.Rect.Width;
                }

                if (temp_y+widget.Size.Height>layoutRect.Y+layoutRect.Height && _wrap)
                {
                    temp_y = y;
                    x += maxOtherSide;
                }
                widget.X = temp_x;
            }

            if (widget is ContainerWidget)
            {
                ContainerWidget w = (ContainerWidget)widget;
                w.UpdateLayout();
            }
        }
    }

    public int GetChildsSideLength(Align align=Align.Horizontal){
        return align == Align.Horizontal ? GetChildsSize(align).Width : GetChildsSize(align).Height;
    }

    public Padding GetPadding() { return _padding; }

    public Size GetChildsSize(Align align=Align.Horizontal)
    {
        int w = 0, h = 0;
        int w_max = _widgets.Count > 0 ? _widgets[0].Rect.Width : 0;
        int h_max = _widgets.Count > 0 ? _widgets[0].Rect.Height : 0;

        foreach (Widget widget in _widgets)
        {
            if (align == Align.Horizontal)
            {
                w += widget.Rect.Width; 
                if (widget.Rect.Height > h_max) h_max = widget.Rect.Height;
            }
            else
            {
                h += widget.Rect.Height;
                if (widget.Rect.Width > w_max) w_max = widget.Rect.Width;
            }
        }
        return align == Align.Horizontal ? new Size(w, h_max) : new Size(w_max, h); 
    }

    public override void Load(ContentManager Content)
    {
        _backgroundImage?.Load(Content);
        foreach (Widget w in _widgets)
        {
            w.Load(Content);
        }
        UpdateLayout();
    }

    public override void Update(GameTime gameTime)
    {
        _backgroundImage?.Update(gameTime);
        base.Update(gameTime);
        UpdateInternal(gameTime, true); 
    }

    public void PrintChildsPos()
    {
        foreach (Widget w in _widgets)
        {
            Console.Write($"[x:{w.Position.X},y:{w.Position.Y}] ");
        }
        Console.WriteLine();
    }

    public void UpdateInternal(GameTime gameTime, bool updateChilds=true) {
        if (updateChilds){
            foreach (Widget w in _widgets)
            {
                w.Update(gameTime);
            }
        }
    }


    public override void Draw(SpriteBatch _spriteBatch)
    {
        Draw(_spriteBatch, drawChilds:true);

    }

    public void Draw(SpriteBatch _spriteBatch, bool drawChilds = true)
    {
        base.Draw(_spriteBatch);
        _border.Draw(_spriteBatch, _rect);
        if (_bgColor != null) Shape.FillRectangle(_spriteBatch, _rect, (Color)_bgColor);
        _backgroundImage?.Draw(_spriteBatch);
        if (drawChilds)
            DrawChilds(_spriteBatch);
    }

    public void DrawChilds(SpriteBatch _spriteBatch)
    {
        foreach (Widget w in _widgets)
        {
            w.Draw(_spriteBatch);
        }
    }
}