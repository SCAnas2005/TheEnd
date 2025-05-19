



using System;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using TheEnd;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public class ImageWidget : Widget
{
    private String _src;
    private Texture2D _texture;
    private float _scale;

    public ImageWidget(
        Rectangle rect, 
        string src = null,
        Texture2D texture = null,
        float scale = 1f,
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick:OnClick, debug: debug)
    {
        _src = src;
        _texture = texture;
        _scale = scale;

        // if (src == null && texture == null)
        //     throw new ArgumentException("You must provide either a texture or a source path (src).");

        _debug = debug;
        this.OnClick = OnClick;
    }

    public ImageWidget(
        Size size, 
        string src = null,
        Texture2D texture = null,
        float scale = 1f,
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0,0,size.Width, size.Height), src, texture, scale, debug, OnClick) 
    {}

    public ImageWidget( 
        string src = null,
        Texture2D texture = null,
        float scale = 1f,
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, src, texture, scale, debug, OnClick) 
    {}

    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
    }}

    public override void Load(ContentManager Content) {
        if (_texture == null && _src != null)
        {
            _texture = Content.Load<Texture2D>(_src);
        }
        if (Size.IsEmpty)
        {
            Size = new Size(_texture.Width, _texture.Height);
        }

        // Scaling image
        
        Size = new Size((int)Math.Round(Size.Width*_scale), (int)Math.Round(Size.Height*_scale));
    }   

    public void Action()
    {
        OnClick?.Invoke();
    }

    public override void Update(GameTime gameTime)
    {
        if(InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            Action();
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
        if (_texture != null)
            _spriteBatch.Draw(_texture, _rect, Color.White);
        // Other scale drawing
        // _spriteBatch.Draw(_texture, Position, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);

    }
}