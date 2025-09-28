

using System;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class KeyItem : Item
{
    public string doorToUnlock;
    public KeyItem(Rectangle rect, string src, string name, string doorToUnlock, Map map, bool dropped = true, bool debug=false) : base(rect, src, name, map, dropped,debug)
    {
        this.doorToUnlock = doorToUnlock;
    }

    public KeyItem() : this(new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), "Items/secret_key", "Key", null, null)
    {
        
    }
    

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
    }


    public void Unlock(NormalDoorObject door)
    {
        if (door.locked && door.name == doorToUnlock)
        {
            door.Unlock();
        }
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
        base.Draw(_spriteBatch);
    }
}