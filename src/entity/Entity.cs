
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public interface IDamageable
{
    void TakeDamage(int damage);
}

public interface IHittable
{
    void OnHit(Sprite sprite);
}

public interface IWeaponUser { }

public abstract class Entity(Rectangle rect, string src, Map map, bool debug = false)
{
    protected Rectangle _rect = rect;
    protected string _src = src;
    protected Texture2D _texture;
    public bool KillMe = false;
    public Map Map = map;
    public bool IsSolid = false;
    public bool debug = debug;

    public int zIndex = 1;


    public virtual Rectangle Rect { get { return _rect; } set { _rect = value; } }
    public virtual Size Size { get { return new Size(_rect.Width, _rect.Height); } }
    public virtual Vector2 Position { get { return new Vector2(_rect.X, _rect.Y); } set { _rect.X = (int)value.X; _rect.Y = (int)value.Y; } }

    public virtual Texture2D Texture { get => _texture; }

    public virtual void Kill()
    {
        KillMe = true;
    }

    public virtual void Load(ContentManager Content)
    {
        _texture = Content.Load<Texture2D>(_src);
    }
    public virtual void Update(GameTime gameTime) { }

    public virtual void UpdateMovement(GameTime gameTime) { }
    public virtual void UpdateOffscreen(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw(_texture, _rect, Color.White);
    }

    public virtual void DrawDebug(SpriteBatch _spriteBatch)
    {
        Shape.DrawRectangle(_spriteBatch, Rect, Color.Red);
    }

}