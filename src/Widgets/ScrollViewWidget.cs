
using System;
using System.Linq;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public enum ScrollDirection{
    Horizontal,
    Vertical
}

public class ScrollViewWidget : Widget
{
    private ContainerWidget _child;
    private ScrollDirection _direction;

    private Size _totalContainerSize;
    private int _scrollOffset;
    private int _previousWheelValue = Mouse.GetState().ScrollWheelValue;

    private int _scrollPosition;
    private int _minScrollPosition;
    private int _maxScrollPosition;


    public ScrollViewWidget(ContainerWidget child, ScrollDirection direction = ScrollDirection.Vertical) : base(Rectangle.Empty) 
    {
        _child = child;
        _direction = direction;
        InitPosition();
        Console.WriteLine($"min scroll position : {_minScrollPosition}, max scroll position: {_maxScrollPosition}");
        Console.WriteLine($"container height : {_child.Rect.Height}, container widgets height : {_totalContainerSize.Height}");

    }

    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
        _child.Rect = _rect;
        InitPosition();
        _child.UpdateLayout();
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
        _child.Rect = _rect;
        InitPosition();
        _child.UpdateLayout(); 
    }}

    public void InitPosition()
    {
        _rect = _child.Rect;
        _totalContainerSize = _child.GetChildsSize(_direction == ScrollDirection.Horizontal ? Align.Horizontal : Align.Vertical);
        _scrollOffset = _child.Rect.Y;
        _minScrollPosition = 0;
        _maxScrollPosition = _direction == ScrollDirection.Vertical ? _totalContainerSize.Height-_child.Rect.Height : _totalContainerSize.Width-_child.Rect.Width;
        _scrollPosition = _minScrollPosition;

    }

    public override void Load(ContentManager Content)
    {
        _child.Load(Content);
    }

    public override void Update(GameTime gameTime)
    {   
        _totalContainerSize = _child.GetChildsSize(_direction == ScrollDirection.Horizontal ? Align.Horizontal : Align.Vertical);
        _child.UpdateInternal(gameTime, updateChilds: false); 
        
        foreach (Widget item in _child._widgets)
        {
            if (_child.Rect.Intersects(item.Rect))
            {
                item.Update(gameTime);
            }
        }

        if (InputManager.Hover(_rect))
        {
            int wheelDelta = Mouse.GetState().ScrollWheelValue;
            int scrollChange = (wheelDelta - _previousWheelValue) / 120 * 20;

            _scrollOffset += scrollChange;
            int maxScroll = Math.Max(0, _maxScrollPosition);
            _scrollOffset = Math.Clamp(_scrollOffset, -maxScroll, 0);

            if (wheelDelta != _previousWheelValue)
            {
                if (_scrollPosition != _scrollOffset)
                {
                    _scrollPosition = _scrollOffset;

                    int newPos = _child.Y + _scrollPosition;
                    _child.UpdateLayout(startX: null, startY: newPos);
                }
            }

            _previousWheelValue = wheelDelta;
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.End();

        RasterizerState rState = new RasterizerState() { ScissorTestEnable = true };
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, rState);

        Globals.Graphics.GraphicsDevice.ScissorRectangle = _rect;

        _child.Draw(_spriteBatch);

        _spriteBatch.End();
        _spriteBatch.Begin();
    }

}