
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public enum BarrelState
{
    Normal,
    TakeDamage
}
public class BarrelEntity : AnimatedEntity, IDamageable
{
    public BarrelEntity(Rectangle rect, Map map) : base(rect, "", map)
    {
        IsSolid = true;
    }

    public BarrelEntity() : this(rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), map:null)
    {
        
    }

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        _animation.AddAnimation("normal");
        _animation.FromSpritesheet("normal", Content.Load<Texture2D>("Entities/Barrel/normal"), new Size(12, 16));

        _animation.AddAnimation("takedamage");
        _animation.FromSpritesheet("takedamage", Content.Load<Texture2D>("Entities/Barrel/normal"), new Size(12, 16));

        _animation.Play("normal");
    } 

    private void Explode()
    {
        var explosion = new ExplosionEntity(Utils.ExpandRect(Rect, 20), Map);
        explosion.Load(Globals.Content);
        // ((GameScene)SceneManager.GetScene(SceneState.Game))._entities.Add(explosion);
        EntityManager.AddEntity(explosion);
        Camera2D.Shake(0.5f, 10f);
        KillMe = true;
    }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        var a = _animation.CurrentAnimation;
        if (a != null && _animation.CurrentAnimationName == "takedamage" && a.IsFinished)
        {
            Explode();
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        base.Draw(_spriteBatch);
        DrawDebug(_spriteBatch);
    }

    public void TakeDamage(int damage)
    {
        _animation.Play("takedamage");
    }
}