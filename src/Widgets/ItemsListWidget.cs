using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheEnd;


public class ItemsListWidget : Widget
{
    private List<Item> _items;
    private Widget _container;
    private Padding padding;

    public ItemsListWidget(
        Rectangle rect,
        List<Item> items,
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick: OnClick, debug: debug)
    {
        _debug = debug;
        this.OnClick = OnClick;

        _items = items;

        padding = new Padding(all: 5);
        _container = new ContainerWidget(
            rect: _rect,
            widgets:[
                new ScrollViewWidget(
                    child: new ContainerWidget(
                        rect: rect,
                        alignItem: Align.Vertical,
                        backgroundColor: Color.White,
                        mainAxisAlignment: MainAxisAlignment.Start,
                        crossAxisAlignment: CrossAxisAlignment.Start,
                        padding: padding,
                        widgets: createItemsWidget().ToArray()
                    )
                )
            ]
        );
    }

    public List<Widget> createItemsWidget()
    {
        List<Widget> w = [];

        Dictionary<String, String> pinfo = [];

        foreach (var item in _items)
        {
            pinfo = [];
            if (item is Weapon we) {
                pinfo["ammo"] = we.Ammo + "";
                pinfo["damage"] = we.Damage + "";
            } else if (item is MedkitItem med) {
                pinfo["heal"] = med.Health + "";
            } else if (item is AmmoItem ammo) {
                pinfo["ammo"] = ammo.Ammo + "";
            } else if (item is KeyItem key) {
                pinfo["door"] = key.doorToUnlock + "";
            } else if (item is PhotoItem p) {
                pinfo["is used"] = p.IsUsed + "";
            }
            Widget a = new ContainerWidget(
                size: new Size(Size.Width - 10, 150),
                backgroundColor: Color.Purple,
                widgets: [
                    new ContainerWidget(
                        border: new Border(all:1, color: Color.White),
                        widgets: [new ImageWidget(size: new Size(100,100), texture: item.Texture)]
                    ),
                    new SizedBox(width: 5),
                    new ContainerWidget(
                        crossAxisAlignment: CrossAxisAlignment.Start,
                        widgets: [
                            new ContainerWidget(
                                size: new Size((Size.Width-100-5-padding.Right)/2, 100),
                                alignItem: Align.Vertical,
                                crossAxisAlignment: CrossAxisAlignment.Start,
                                border: new Border(all: 1, color: Color.White),
                                widgets: [
                                    new TextWidget($"name: {item.Name}"),
                                    new TextWidget($"position: {item.Position}"),
                                    new TextWidget($"is droped: {item.IsDropped}"),
                                    new TextWidget($"mapscene: {item.Map.Scene}"),
                                ]
                            ),
                            new ContainerWidget(
                                size: new Size((Size.Width-100-5-padding.Right)/2, 100),
                                alignItem: Align.Vertical,
                                crossAxisAlignment: CrossAxisAlignment.Start,
                                border: new Border(all: 1, color: Color.White),
                                widgets: pinfo.Select(info => new TextWidget($"{info.Key}: {info.Value}")).ToArray()
                            ),
                        ]
                    ),
                ]
            );

            w.Add(a);
        }

        return w;
    }

    public ItemsListWidget(
        Size size,
        List<Item> items,
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0, 0, size.Width, size.Height), items, debug, OnClick)
    { }

    public ItemsListWidget( 
        List<Item> items,
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, items, debug, OnClick) 
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
        if (InputManager.LeftClicked && InputManager.Hover(_rect))
        {
            Action();
        }

        _container.Update(gameTime);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _container.Draw(_spriteBatch);
    }
}