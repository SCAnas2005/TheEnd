
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class FogEntity : Entity, IHittable
{
    private int _damage;
    private float _cooldownSeconds = 0.5f;
    private float COOLDOWN_RATE = 500;
    private Dictionary<Sprite, double> _lastHitTimes = new();


    public FogEntity(Rectangle rect, Map map, bool debug = false) : base(rect, "Entities/green_fog", map, debug)
    {
        _damage = 30;
        zIndex = 10;
    }

    public FogEntity() : this(rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(50, 20).ToPoint()), map: null)
    {
        
    }



    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // var _sprites = gameScene._entities.OfType<Sprite>().ToList();
        var _sprites = EntityManager.GetAll<Sprite>();

        var currentTime = gameTime.TotalGameTime.TotalSeconds;

        foreach (var s in _sprites)
        {
            if (Rect.Intersects(s.Rect))
            {
                if (s is Player p)
                {
                    var g = p.Inventory.GetFirst<GasMaskItem>();
                    if (g != null && g.IsWearing) continue;
                }
                if (!_lastHitTimes.ContainsKey(s) || currentTime - _lastHitTimes[s] >= _cooldownSeconds)
                {
                    OnHit(s);
                    _lastHitTimes[s] = currentTime;
                }
            }
        }
    }

    public void OnHit(Sprite sprite)
    {
        sprite.TakeDamage(_damage);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw(_texture, Rect, Color.White * 0.5f);
    }
}