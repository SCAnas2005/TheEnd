
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BookItem : Item
{
    public BookStruct Content;


    public BookItem(Rectangle rect, string src, string name, Map map, bool dropped = true, bool debug = false) : base(rect, src, name, map, dropped, debug: debug)
    {
        EditAction(
            "pick",
            conditionToShow: (p) => IsDropped,
            conditionToAction: (p) => IsDropped && p.CanUseItem && IsIntersectWithPlayer && p.CanMove,
            action: (player) =>
            {
                if (!player.Inventory.IsFull)
                {
                    IsDropped = false;
                    player.Inventory.AddItem(this);
                }
            }
        );
    }

    public BookItem() : this(new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), src: null, name: "Book", map: null, debug: true)
    {

    }

    public override void Update(GameTime gameTime)
    {
        Player player = EntityManager.Player;

        if (!IsDropped)
        {
            Position = player.Position;

            if (InputManager.IsPressed(Microsoft.Xna.Framework.Input.Keys.J) && player.CanMove && player.CanUseItem)
            {
                if (BookObjectView.Instance == null)
                {
                    BookObjectView.Instance = new BookObjectView();
                    BookObjectView.Instance.Book = Content;
                    BookObjectView.Instance.Show();
                }
                else
                {
                    BookObjectView.Instance.Close();
                    BookObjectView.Instance = null;

                }
            }
        }

        BookObjectView.Instance?.Update(gameTime);
        base.Update(gameTime);
    }

    public override void OnDrop()
    {
        BookObjectView.Instance.Close();
        BookObjectView.Instance = null;
        base.OnDrop();
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        BookObjectView.Instance?.Draw(_spriteBatch);
        base.Draw(_spriteBatch);
    }
}