

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class AmmoItem : Item
{
    private int _ammo;
    
    public int Ammo { get { return _ammo; } set{ _ammo = value; }}
    public AmmoItem(Rectangle rect, string name, Map map, int ammo = 30, bool dropped = true, bool debug = false) : base(rect, "Items/ammo-gun", name, map, dropped, debug: debug)
    {
        _ammo = ammo;

        EditAction(
            "pick",
            action: (player) =>
            {
                if (IsDropped)
                {
                    IsDropped = false;
                    if (player.Inventory.Has(typeof(Gun)))
                    {
                        Gun w = player.Inventory.GetFirst<Gun>();
                        w.AddAmmo(_ammo);
                        KillMe = true;
                    }
                    else
                    {
                        if (player.Inventory.Has(typeof(AmmoItem)))
                        {
                            AmmoItem a = player.Inventory.GetFirst<AmmoItem>();
                            a.Ammo += Ammo;
                            KillMe = true;
                        }
                        else
                        {
                            player.Inventory.AddItem(this);
                        }
                    }
                }
            }
        );
    }


    // Constructeur par défaut
    public AmmoItem() : this(new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), "Ammo", null, 30, true, false)
    {

    }

    

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
    }


    public override void Update(GameTime gameTime)
    {
        Player player = EntityManager.Player;
        if (!IsDropped)
        {
            Position = player.Position;
        }
        base.Update(gameTime);
    }


    public override void Draw(SpriteBatch _spriteBatch)
    {
        base.Draw(_spriteBatch);
    }
}