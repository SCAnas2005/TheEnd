
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public enum ExplosionState
{
    Explosion
}

public class ExplosionEntity : AnimatedEntity
{
    private bool _soundPlayed = false;
    private SoundEffect _explosionSound;
    public ExplosionEntity(Rectangle rect, Map map) : base(rect, "", map)
    {
        zIndex = 10;
    }

    public ExplosionEntity() : this(rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(30).ToPoint()), map: null)
    {
        
    }

    public bool IsFinished { get => _animation.CurrentAnimation != null ? _animation.CurrentAnimation.IsFinished : false; }


    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        _animation.AddAnimation("explosion").FramePerSeconds = 20;
        _animation.FromSpritesheet("explosion", Content.Load<Texture2D>("Entities/explosion_medium"), new Size(200), offset: new Rectangle(x: 50, y: 50, width: -100, height: -125));

        _animation.Get("explosion").Looping = false;
        _animation.Play("explosion");


        _explosionSound = Content.Load<SoundEffect>("sounds/entities/explosion/explosion-small");
    }



    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!_soundPlayed)
        {
            AudioManager.Play(_explosionSound);
            _soundPlayed = true;
        }
        if (IsFinished)
        {
            KillMe = true;
        }
        else
        {
            var zombies = ZombieManager.Zombies;
            var player = EntityManager.Player;
            // var entities = ((GameScene)SceneManager.GetScene(SceneState.Game))._entities.Where(e => e is IDamageable).ToList();
            var entities = EntityManager.GetEntitiesOfType<IDamageable>();

            if (Rect.Intersects(player.Rect)) { player.TakeDamage(500); }
            foreach (var z in zombies)
            {
                if (Rect.Intersects(z.Rect)) { z.TakeDamage(500); }
            }
            foreach (var e in entities)
            {
                if (Rect.Intersects(e.Rect)) { ((IDamageable)e).TakeDamage(500); }
            }
        }
    }
}