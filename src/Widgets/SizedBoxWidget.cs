
using System;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;




public class SizedBox : Widget
{
    private Color? _bgColor;
    private Widget _child;
    
    public SizedBox(Rectangle rect, Color? backgroundColor = null, Widget child=null, bool debug=false) : base(rect, debug:debug) {
        _bgColor = backgroundColor;
        _child = child;
        UpdateLayout();
    }

    public override int X {get {return _rect.X;} set {_rect.X = value; UpdateLayout();}}
    public override int Y {get {return _rect.Y;} set {_rect.Y = value; UpdateLayout();}}

    public SizedBox(Size size, Color? backgroundColor = null, Widget child=null, bool debug=false) : this(new Rectangle(0,0,size.Width, size.Height), backgroundColor, child:child, debug:debug) {
    }

    public SizedBox(int width = 0, int height = 0, Color? backgroundColor = null, Widget child=null, bool debug=false) : this(new Size(width, height), backgroundColor, child:child, debug:debug) {}

    public override void Load(ContentManager Content) {
        _child?.Load(Content);
    }

    public override void Update(GameTime gameTime) {
        base.Update(gameTime);

        _child?.Update(gameTime);
    }

    public override void Draw(SpriteBatch _spriteBatch) {
        base.Draw(_spriteBatch);
        if (_bgColor != null)
        {
            Shape.FillRectangle(_spriteBatch, _rect, (Color)_bgColor);
        }

        _child?.Draw(_spriteBatch);
    }

    public void UpdateLayout()
    {
        if (_child == null) return;
        
        if (_child is TextWidget)
        {
            _child.Rect = Rect;
        }
    }

}