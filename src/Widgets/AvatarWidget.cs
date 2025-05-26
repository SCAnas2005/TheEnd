
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


public class AvatarWidget : Widget
{

    private ImageWidget _image;
    private TextWidget _text;
    private ContainerWidget _container;

    public AvatarWidget(
        Rectangle rect,
        ImageWidget image,
        TextWidget text,
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick: OnClick, debug: debug)
    {
        _debug = debug;
        this.OnClick = OnClick;

        _image = image;
        _text = text;


        _container = new ContainerWidget(
            rect: rect,
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Center,
            widgets:[
                _image,
                new SizedBox(width: 10),
                _text,
            ]
        );
    }

    public AvatarWidget(
        Size size, 
        ImageWidget image,
        TextWidget text,
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0,0,size.Width, size.Height), image, text, debug, OnClick) 
    {}

    public AvatarWidget( 
        ImageWidget image,
        TextWidget text,
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, image, text, debug, OnClick) 
    {}

    public override int X {get {return _rect.X;} set {
        _rect.X = value; _container.X = _rect.X;
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; _container.Y = _rect.Y;
    }}

    public override void Load(ContentManager Content)
    {
        _container.Load(Content);
    }   

    public void Action()
    {
        if (OnClick != null)
            OnClick();
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            Action();
        }

        _container.Update(gameTime);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _container.Draw(_spriteBatch);
        if (_debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
    }
}