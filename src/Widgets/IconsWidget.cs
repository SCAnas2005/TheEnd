
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class IconWidget : Widget
{
    private Icons _icon;
    private string _src;
    private Texture2D _texture;
    private Color _color;


    public Icons Icon {get {return _icon;} set {
        if (_icon != value)
        { 
            _icon = value; 
            _texture = GetTextureFromIcon(_icon);
        }
    }}


    public IconWidget(Rectangle rect, string src, Color? color = null, bool debug = false) : base(rect, debug: debug) {
        _src = src;
        _color = color ?? Color.White;
    }

    public IconWidget(Size size, string src, Color? color = null, bool debug = false) : this(new Rectangle(0,0,size.Width, size.Height), src, color, debug) {

    }

    public IconWidget(Rectangle rect, Icons icon, Color? color = null, bool debug = false) : base(rect, debug: debug) {
        _icon = icon;
        _src = IconsManager.GetIconPath(_icon);
        _color = color ?? Color.White;      
    }

    public IconWidget(Size size, Icons icon, Color? color = null, bool debug = false) : this(new Rectangle(0,0,size.Width, size.Height), icon, color, debug) 
    {}

    public Texture2D GetTextureFromIcon(Icons icon)
    {
        return Globals.Content.Load<Texture2D>(IconsManager.GetIconPath(icon));
    }

    public override void Load(ContentManager Content)
    {
        _texture = GetTextureFromIcon(_icon);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
        _spriteBatch.Draw(_texture, _rect, _color);
    }

}