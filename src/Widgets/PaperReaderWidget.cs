

using System;
using System.ComponentModel;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


public class PaperReaderWidget : Widget
{
    private string _text;
    public ContainerWidget _container;

    public PaperReaderWidget(
        Rectangle rect,
        string text = "",
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick: OnClick, debug: debug)
    {
        _text = text;
        Size s = new Size(Utils.GetValueByPercentage(_rect.Width, 90), Utils.GetValueByPercentage(_rect.Height, 90));
        _container = new ContainerWidget(
            rect: _rect,
            mainAxisAlignment: MainAxisAlignment.Center,
            crossAxisAlignment: CrossAxisAlignment.Center,
            backgroundImage: new ImageWidget(src: "Items/paper_panel"),
            padding: new Padding(all: 20),
            widgets: [
                new ScrollViewWidget(
                    child: new ContainerWidget(
                        rect: _rect,
                        mainAxisAlignment: MainAxisAlignment.Start,
                        crossAxisAlignment: CrossAxisAlignment.Center,
                        alignItem: Align.Vertical,
                        widgets: [
                            new SizedBox(
                                size: new Size(s.Width,s.Height),
                                child: new TextWidget(_text, color: Color.Black, font: CFonts.Minecraft_24),
                                debug:false
                            ),
                        ]
                    )
                )
            ]
        );
    }

    public PaperReaderWidget(
        Size size, 
        string text = "",
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0,0,size.Width, size.Height), text, debug, OnClick) 
    {}

    public PaperReaderWidget( 
        string text = "",
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, text, debug, OnClick) 
    {}

    public override int X {get {return _rect.X;} set {
        _rect.X = value; 
    }}
    public override int Y {get {return _rect.Y;} set {
        _rect.Y = value; 
    }}

    public override void Load(ContentManager Content) {
        _container.Load(Content);
    }   

    public void Action()
    {
        if (OnClick != null)
            OnClick();
    }

    public override void Update(GameTime gameTime)
    {
        if(InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            Action();
        }
        _container.Update(gameTime);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.End();
        _spriteBatch.Begin();
        _container.Draw(_spriteBatch);
        base.Draw(_spriteBatch);
        _spriteBatch.End();
        SpriteBatchContext.ApplyToContext(_spriteBatch, SpriteBatchContext.Top);
    }
}