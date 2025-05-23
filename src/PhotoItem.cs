
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class PhotoItem : Item
{
    private Texture2D _photoTexture;
    private Texture2D _cadreTexture;
    public bool IsUsed;
    public PhotoItem(Rectangle rect, string src, string name, Map map, MapScene mapScene, bool dropped = true) : base(rect, src, name, map, mapScene, dropped)
    {
        IsUsed = false;
    }

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        _photoTexture = Content.Load<Texture2D>("Items/photo");
        _cadreTexture = Content.Load<Texture2D>("Items/paper_panel_2");
    }


    public override void Action(Player player, Map map)
    {
        if (IsDropped)
        {
            if (!player.Inventory.IsFull)
            {
                IsDropped = false;
                player.Inventory.AddItem(this);
            }
        }
        else
        {
            IsUsed = !IsUsed;
        }
        Console.WriteLine("test");
    }


    public override void Update(GameTime gameTime, Player player, Map map)
    {
        if (!IsDropped)
        {
            Rect = new Rectangle(player.Rect.X, player.Rect.Y, Rect.Width, Rect.Height);   
        }
        base.Update(gameTime, player, map);
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
            }
        }
        base.DrawIndications(_spriteBatch);
    }
}