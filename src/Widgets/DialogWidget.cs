
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class DialogWidget : StatefulWidget
{
    private ContainerWidget _container;
    private string _text;
    private string _name;
    private Texture2D _profilePicture;
    public DialogWidget(Rectangle rect, string name, string text, Texture2D profilePicture = null, Action OnClick = null, Action OnHover = null, Action OnNotHover = null, bool debug = false) : base(rect: rect, debug: debug, OnClick: OnClick)
    {
        _name = name;
        _text = text;
        _profilePicture = profilePicture;

        Build();
    }

    public override void Build()
    {
        Padding p = new Padding(5);
        _container = new ContainerWidget(
            rect: Rect,
            alignItem: Align.Vertical,
            padding: p,
            crossAxisAlignment: CrossAxisAlignment.Start,
            border: new Border(1, color: Color.Indigo),
            backgroundColor: Color.Blue * 0.5f,
            widgets: [
                new SizedBox(width: Size.Width-p.Left*2, height: 50, child: new TextWidget(_name, font: CFonts.Minecraft_24)),
                new ContainerWidget(
                    size: new Size(Size.Width-p.Left*2, Size.Height-50-p.Top*2),
                    crossAxisAlignment: CrossAxisAlignment.Start,

                    widgets: [
                        new ContainerWidget(
                            size: new Size(Size.Height-50-p.Top*2, Size.Height-50-p.Top*2),
                            // backgroundColor: Color.Green,
                            widgets:[
                                _profilePicture != null ?
                                    new ImageWidget(size: new Size(Size.Height-50-p.Top*2, Size.Height-50-p.Top*2), texture: _profilePicture)
                                :
                                    new ContainerWidget()
                            ]
                        ),
                        new ContainerWidget(
                            size: new Size(Size.Width-p.Left*2-(Size.Height-50-p.Top*2), Size.Height-50-p.Top*2),
                            mainAxisAlignment: MainAxisAlignment.Center,
                            crossAxisAlignment: CrossAxisAlignment.Center,
                            // backgroundColor: Color.Pink,
                            padding: new Padding(10),
                            widgets: [
                                new SizedBox(
                                    size: new Size(Size.Width-p.Left*2-(Size.Height-50-p.Top*2)-20, Size.Height-50-p.Top*2-20),
                                    child: new TextWidget(_text, font: CFonts.Minecraft_24)
                                )
                            ]
                        )
                    ]
                ),
            ]
        );
        if (_loaded)
        {
            Load(Globals.Content);
        }
    }

    public override void Load(ContentManager Content)
    {
        _container.Load(Content);
        _loaded = true;
    }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _container.Update(gameTime);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.End();
        _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
        base.Draw(_spriteBatch);
        _container.Draw(_spriteBatch);

        _spriteBatch.End();
        var context = SpriteBatchContext.Top;
        SpriteBatchContext.ApplyToContext(_spriteBatch, context);
    }
}