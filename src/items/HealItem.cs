

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class HealItem : Item
{
    protected int _health;
    public int Health { get { return _health; } set{ _health = value; }}

    public HealItem(Rectangle rect, string src, string name, Map map, int health = 100, bool dropped = true, bool debug = false) : base(rect, src, name, map, dropped, debug: debug)
    {
        _health = health;

        EditAction("pick", action: (player) =>
            {
                if (IsDropped)
                {
                    IsDropped = false;
                    player.Inventory.AddItem(this);
                }
                else
                {
                    player.AddHealth(Health);
                    player.Inventory.RemoveItem(this);
                    KillMe = true;
                }
            }
        );
    }


    public override void Update(GameTime gameTime)
    {
        Player player = EntityManager.Player;
        base.Update(gameTime);
        if (!IsDropped)
        {
            Rect = new Rectangle(player.Rect.X, player.Rect.Y, Rect.Width, Rect.Height);
        }
    }



    public override void Draw(SpriteBatch _spriteBatch)
    {
        base.Draw(_spriteBatch);
    }
}