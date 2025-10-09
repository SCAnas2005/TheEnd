
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

public static class Directions
{
    public static Vector2 right = new Vector2(1, 0);
    public static Vector2 left = new Vector2(-1, 0);
    public static Vector2 down = new Vector2(0, 1);
    public static Vector2 up = new Vector2(0, -1);
}

public interface IItemUser
{
    
}

public abstract class Sprite : Entity
{
    protected int _maxHealth;
    protected int _health;

    protected float _speed;
    protected bool _isDead;

    protected Vector2 _direction;
    protected Vector2 Direction { get => _direction; }
    protected Vector2 Velocity;

    public bool IsDead { get { return _isDead; } set { _isDead = value; } }


    public int Health { get { return _health; } set { _health = value; } }
    public int MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }

    public bool CanMove = true;

    public float Speed { get { return _speed; } set { _speed = value; } }
    public bool IsCollision;

    public bool IsCollisionLeft;
    public bool IsCollisionRight;
    public bool IsCollisionUp;
    public bool IsCollisionDown;

    protected (int, int) _mapPos;

    public string Name;

    public Texture2D ProfilePicture = null;

    protected List<Vector2> _movePath = null;
    public List<Vector2> MovePath
    {
        get { return _movePath; }
        set
        {
            _movePath = value;
            _currentIndexPath = 0;
        }
    }
    public Vector2 SpawnPoint;
    protected int _currentIndexPath = 0;

    protected AnimatedSprite2D animation;



    public Sprite(Rectangle rect, string src, float speed, int health, Map map, string name = "", bool debug = false) : base(rect: rect, src: src, map: map, debug: debug)
    {
        animation = new AnimatedSprite2D();
        _maxHealth = health;
        _health = health;
        _speed = speed;

        IsCollision = false;
        SetColisionDirection(); // All False

        _isDead = false;

        _direction = Vector2.Zero;
        Velocity = Vector2.Zero;

        Name = name;
    }

    public static Size GetSpriteSize()
    {
        return new Size(16) - new Size(3);
    }

    public void CreateBaseAnimations()
    {
        animation.AddAnimation("idle");
        animation.AddAnimation("move_right");
        animation.AddAnimation("move_up");
        animation.AddAnimation("move_down");
    }

    public List<(int, int)> GetMapPositions(Map map)
    {
        // Return the (l, c) pos that the player is on.
        List<(int, int)> pos = [];
        (int startRow, int startCol) = Map.GetMapPosFromVector2(new Vector2(_rect.X, _rect.Y), map.TileSize);
        (int endRow, int endCol) = Map.GetMapPosFromVector2(new Vector2(_rect.X + _rect.Width, _rect.Y + _rect.Height), map.TileSize);
        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                pos.Add((row, col));
            }
        }
        return pos;
    }

    public void SetColisionDirection(bool left = false, bool right = false, bool up = false, bool down = false)
    {
        IsCollisionLeft = left; IsCollisionRight = right; IsCollisionUp = up; IsCollisionDown = down;
    }


    public bool CheckCollision(Map map, Rectangle rect)
    {
        if (rect.X < map.Rect.X || rect.X + rect.Width >= map.Rect.X + map.Rect.Width || rect.Y < map.Rect.Y || rect.Y + rect.Height >= map.Rect.Y + map.Rect.Height)
        {
            return IsCollision = true;
        }

        // List<Entity> entities = ((GameScene)SceneManager.GetScene(SceneState.Game))._entities.Where(e => e.IsSolid && e.Map == Map).ToList();
        var entities = EntityManager.GetEntitiesWhere((e) => e.IsSolid && e.Map == map);
        foreach (var ent in entities)
        {
            if (ent.Rect.Intersects(rect))
            {
                return IsCollision = true;
            }
        }


        if (!map.TiledMap.Layers.Any(layer => layer.Name == "obstacles")) return false;

        var collisionLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("obstacles");

        // Calculer la position de la tuile en ligne et colonne pour chaque coin du personnage
        (int startRow, int startCol) = _mapPos = Map.GetMapPosFromVector2(new Vector2(rect.X, rect.Y), map.TileSize);

        (int endRow, int endCol) = Map.GetMapPosFromVector2(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), map.TileSize);

        // Vérifier toutes les tuiles entre startRow/endRow et startCol/endCol
        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                TiledMapTile? tile;

                if (collisionLayer.TryGetTile((ushort)col, (ushort)row, out tile))
                {

                    var id = tile.Value.GlobalIdentifier;

                    // Si la tuile n'est pas vide (GlobalIdentifier != 0), il y a une collision
                    if (id != 0)
                    {
                        return IsCollision = true;
                    }
                }
            }
        }

        return IsCollision = false;
    }

    public Vector2 FollowPath(float dt)
    {
        Vector2 v = Vector2.Zero;
        if (MovePath != null && _currentIndexPath < MovePath.Count)
        {
            Vector2 target = MovePath[_currentIndexPath] - new Vector2(Map.TileSize.Width / 2, Map.TileSize.Height / 2);
            Vector2 direction = target - Position;
            float distance = direction.Length();
            float threshold = 4f;
            if (distance < threshold) _currentIndexPath++;
            else
            {
                direction.Normalize();
                v.X = direction.X * _speed * dt;
                v.Y = direction.Y * _speed * dt;
            }

            if (_currentIndexPath >= MovePath.Count)
            {
                Console.WriteLine("Follow path finished");
                MovePath = null;
            }
        }
        return v;
    }

    public void TryMove(Map map, Vector2 velocity)
    {
        // Déplacement horizontal
        // Console.WriteLine("Rect collision: "+ CheckCollision(Map, _rect));
        float stepX = Math.Sign(velocity.X);
        for (int i = 0; i < Math.Abs(velocity.X); i++)
        {
            var testRect = Utils.AddToRect(_rect, new Rectangle((int)stepX, 0, 0, 0));
            if (!CheckCollision(map, testRect))
            {
                _rect.X += (int)stepX;
            }
            else
            {
                if (velocity.X > 0) SetColisionDirection(right: true, up: IsCollisionUp, down: IsCollisionDown);
                else if (velocity.X < 0) SetColisionDirection(left: true, up: IsCollisionUp, down: IsCollisionDown);
                else { SetColisionDirection(false); }
                break;
            }

        }

        // Déplacement vertical
        float stepY = Math.Sign(velocity.Y);
        for (int i = 0; i < Math.Abs(velocity.Y); i++)
        {
            var testRect = Utils.AddToRect(_rect, new Rectangle(0, (int)stepY, 0, 0));
            if (!CheckCollision(map, testRect))
            {
                _rect.Y += (int)stepY;
            }
            else
            {
                if (velocity.Y > 0) SetColisionDirection(down: true, right: IsCollisionRight, left: IsCollisionLeft);
                else if (velocity.Y < 0) SetColisionDirection(up: true, right: IsCollisionRight, left: IsCollisionLeft);
                else { SetColisionDirection(false); }
                break;
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
    }


    // public abstract void Load(ContentManager Content);

    // public abstract void Update(GameTime gameTime);

    // public virtual void Draw(SpriteBatch _spriteBatch)
    // {
    //     if (debug)
    //     {
    //         Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
    //     }
    // }

}