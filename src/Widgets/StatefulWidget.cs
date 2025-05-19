



using System;
using TheEnd;
using Microsoft.Xna.Framework;


public abstract class StatefulWidget : Widget
{
    private bool _dirty = true;  
    protected bool _loaded = false;

    public StatefulWidget(
        Rectangle rect, 
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick:OnClick, debug: debug)
    {
        _debug = debug;
        this.OnClick = OnClick;
        _loaded = false;
    }

    public StatefulWidget(
        Size size, 
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0,0,size.Width, size.Height), debug, OnClick) 
    {}

    public StatefulWidget( 
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, debug, OnClick) 
    {}

    public void SetState(Action fn = null)
    {
        fn?.Invoke();
        _dirty = true;
    }

    public abstract void Build();

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (_dirty)
        {
            Build(); // Reconstruit le widget
            _dirty = false;
        }
    }   
}