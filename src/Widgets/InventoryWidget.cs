
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

public record InventoryState(Item[] Items, int SelectedIndex)
{
    public static InventoryState FromInventory(Inventory inventory)
    {
        // Copie défensive
        return new InventoryState(inventory.Items.ToArray(), inventory.SelectedItemIndex);
    }

    public bool IsDifferentFrom(InventoryState other)
    {
        if (SelectedIndex != other.SelectedIndex)
            return true;

        if (Items.Length != other.Items.Length)
            return true;

        for (int i = 0; i < Items.Length; i++)
        {
            if (!ReferenceEquals(Items[i], other.Items[i]))
                return true;
        }

        return false;
    }
}




public class InventoryWidget : StatefulWidget
{
    private InventoryState _lastState;
    private Inventory _inventory;
    private ContainerWidget _container;
    private Size boxSize;

    private Texture2D _cursor;
    public InventoryWidget(
        Rectangle rect,
        Inventory inventory,
        bool debug = false,
        Action OnClick = null
        ) : base(rect, OnClick: OnClick, debug: debug)
    {
        _inventory = inventory;
        _lastState = InventoryState.FromInventory(_inventory);
        boxSize = new Size(70, 70);
        Build();
    }

    public InventoryWidget(
        Size size,
        Inventory inventory,
        bool debug = false,
        Action OnClick = null
        ) : this(new Rectangle(0, 0, size.Width, size.Height), inventory, debug, OnClick)
    { }

    public InventoryWidget(
        Inventory inventory,
        bool debug = false,
        Action OnClick = null
        ) : this(Rectangle.Empty, inventory, debug, OnClick)
    { }

    public override int X
    {
        get { return _rect.X; }
        set
        {
            _rect.X = value;
        }
    }
    public override int Y
    {
        get { return _rect.Y; }
        set
        {
            _rect.Y = value;
        }
    }

    public override void Build()
    {
        List<Widget> w = [];
        int i = 1;
        Console.WriteLine("-------------------");
        _inventory.PrintInventory();
        Console.WriteLine("-------------------");
        foreach (var item in _inventory.Items)
        {
            w.Add(
                new ContainerWidget(
                    alignItem: Align.Vertical,
                    widgets: [
                        new ContainerWidget(
                            size: boxSize,
                            padding: new Padding(all: 10),
                            border: new Border(all: 2, color: Color.Blue),
                            // fait en sorte que inventory puisse avoir des objets vides aussi (null)
                            widgets: [
                                item != null ? new ImageWidget(size: new Size(50,50),texture: item.Texture) : new SizedBox(),
                            ]
                        ),
                        new TextWidget($"[{i}]"),
                    ]
                )
            );
            i++;
        }
        _container = new ContainerWidget(
            rect: _rect,
            alignItem: Align.Horizontal,
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Start,
            debug: true,
            widgets: w.ToArray()
        );
    }

    public override void Load(ContentManager Content)
    {
        _cursor = Content.Load<Texture2D>("Inventory/cursor");
        _container.Load(Content);
        _loaded = true;
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

        var currentState = InventoryState.FromInventory(_inventory);
        if (_lastState.IsDifferentFrom(currentState))
        {
            Console.WriteLine("rebuild of inventory");
            _lastState = currentState;
            SetState();
        }
        _container.Update(gameTime);
        base.Update(gameTime);
    }



    public override void Draw(SpriteBatch _spriteBatch)
    {
        _container.Draw(_spriteBatch);
        _spriteBatch.Draw(_cursor, new Rectangle(_rect.X + (_inventory.SelectedItemIndex * boxSize.Width), _rect.Y, boxSize.Width, boxSize.Height), Color.White);
        base.Draw(_spriteBatch);
    }
}