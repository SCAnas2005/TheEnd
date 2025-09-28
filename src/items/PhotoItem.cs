
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class PhotoItem : Item
{
    private Texture2D _photoTexture;
    private Texture2D _cadreTexture;
    public bool IsUsed;
    public PhotoItem(Rectangle rect, string src, string name, Map map, bool dropped = true, bool debug = false) : base(rect, src, name, map, dropped, debug: debug)
    {
        IsUsed = false;

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

        AddAction(
            "use",
            a: new ActionInteraction(
                name: "Afficher", description: "Afficher la photo", key: Microsoft.Xna.Framework.Input.Keys.J,
                conditionToShow: (p) => true,
                conditionToAction: (player) => !IsDropped && player.CanUseItem && player.CanMove,
                action: (player) => IsUsed = !IsUsed
            )
        );
    }

    public PhotoItem() : this(new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), "family/timmy", "Photo", null)
    {
        
    }

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        _photoTexture = Content.Load<Texture2D>("Items/photo");
        _cadreTexture = Content.Load<Texture2D>("Items/paper_panel_2");
    }


    public override void OnDrop()
    {
        base.OnDrop();
        IsUsed = false;
    }

    public override void OnChange()
    {
        base.OnChange();
        IsUsed = false;
    }



    public override void Update(GameTime gameTime)
    {
        Player player = EntityManager.Player;
        
        if (!IsDropped)
        {
            Rect = new Rectangle(player.Rect.X, player.Rect.Y, Rect.Width, Rect.Height);
        }
        base.Update(gameTime);
    }



    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (IsDropped)
        {
            base.Draw(_spriteBatch);
        }
        else
        {
            if (IsUsed)
            {

                _spriteBatch.End();
                _spriteBatch.Begin();

                _spriteBatch.Draw(_cadreTexture, new Rectangle(100, 100, 600, 600), Color.White);
                _spriteBatch.Draw(_texture, new Rectangle(150, 150, 500, 500), Color.White);

                _spriteBatch.End();
                SpriteBatchContext.ApplyToContext(_spriteBatch, SpriteBatchContext.Top);
            }
        }
        base.DrawIndications(_spriteBatch);
    }
}