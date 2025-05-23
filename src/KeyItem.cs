

using System;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class KeyItem : Item
{
    public string doorToUnlock;
    public KeyItem(Rectangle rect, string src, string name, string doorToUnlock, Map map, MapScene mapScene, bool dropped = true) : base(rect, src, name, map, mapScene, dropped)
    {
        this.doorToUnlock = doorToUnlock;
    }

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
    }


    public override void Action(Player player, Map map)
    {
        if (IsDropped && !player.Inventory.IsFull)
        {
            IsDropped = false;
            player.Inventory.AddItem(this);
        }
    }

    public void Unlock(NormalDoorObject door)
    {
        if (door.locked && door.name == doorToUnlock)
        {
            door.Unlock();
        }
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
        base.Draw(_spriteBatch);
    }
}