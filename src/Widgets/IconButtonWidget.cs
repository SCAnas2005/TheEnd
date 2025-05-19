
using System;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class IconButtonWidget : ButtonWidget
{


    public IconButtonWidget(Rectangle rect, string src, Color? color = null, bool hover = false, Action OnClick = null, bool debug = false) : base(rect, backgroundAsset: src, text:null, hover:hover,backgroundHoverAsset:null, OnClick:OnClick, debug: debug) {
    }

    public IconButtonWidget(Size size, string src, Color? color = null, bool hover = false, Action OnClick = null, bool debug = false) : this(new Rectangle(0,0,size.Width, size.Height), src, color, hover, OnClick, debug) {

    }

    public IconButtonWidget(Rectangle rect, Icons icon, Color? color = null, bool hover = false, Action OnClick = null, bool debug = false) : base(rect, IconsManager.GetIconPath(icon), text:null, hover:hover,backgroundHoverAsset:null, OnClick:OnClick, debug: debug) {
    }

    public IconButtonWidget(Size size, Icons icon, Color? color = null, bool hover = false, Action OnClick = null, bool debug = false) : this(new Rectangle(0,0,size.Width, size.Height), icon, color, hover, OnClick, debug) 
    {}


    // public override void Load(ContentManager Content)
    // {
    //     _texture = Content.Load<Texture2D>(_src);
    // }

    // public override void Update(GameTime gameTime)
    // {
        
    // }

    // public override void Draw(SpriteBatch _spriteBatch)
    // {
    //     _spriteBatch.Draw(_texture, _rect, _color);
    // }

}