using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

public abstract class Weapon : Item
{
    protected int _damage = 10;
    public int Damage { get => _damage; }
    protected float _rotation;
    protected Vector2 _direction;
    public Vector2 Direction { get => _direction; set {_direction = value;}}
    protected List<Bullet> _bullets = new();
    protected SoundEffect _shotSound;
    protected SoundEffect _impactOnMetalSound;
    protected SoundEffect _selectWeaponSound;

    public int Ammo { get; protected set; } = 10;
    public bool IsShooting { get; protected set; }
    public IWeaponUser Owner { get; protected set; }

    public Weapon(Rectangle rect, string src, string name, Map map, IWeaponUser owner,
                  int dx = 1, bool dropped = true, bool debug = false)
        : base(rect, src, name, map, dropped, debug)
    {
        _direction = new Vector2(dx, 0);
        Owner = owner;
        IsDropped = dropped;
    }

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        // les sons seront chargés par les classes filles
    }


    public override void OnDrop()
    {
        base.OnDrop();
        Owner = null;
        IsDropped = true;
    }


    public abstract void Shoot();

    public bool CheckCollision(Map map, Bullet bullet)
    {
        if (!map.TiledMap.Layers.Any(layer => layer.Name == "obstacles"))
            return false;

        var collisionLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("obstacles");

        Vector2 start = bullet.PreviousPosition;
        Vector2 end = bullet.Position;

        // Convertit en indices de tuile (start et end)
        (int startRow, int startCol) = Map.GetMapPosFromVector2(start, map.TileSize);
        (int endRow, int endCol) = Map.GetMapPosFromVector2(end, map.TileSize);

        // Détermine la zone de tuiles à vérifier
        int minRow = Math.Min(startRow, endRow);
        int maxRow = Math.Max(startRow, endRow);
        int minCol = Math.Min(startCol, endCol);
        int maxCol = Math.Max(startCol, endCol);

        for (int row = minRow; row <= maxRow; row++)
        {
            for (int col = minCol; col <= maxCol; col++)
            {
                if (collisionLayer.TryGetTile((ushort)col, (ushort)row, out var tile))
                {
                    if (tile.Value.GlobalIdentifier != 0)
                    {
                        // BoundingBox du tile
                        Rectangle tileRect = new Rectangle(
                            col * map.TileSize.Width,
                            row * map.TileSize.Height,
                            map.TileSize.Width,
                            map.TileSize.Height
                        );

                        // Test segment vs tile
                        if (Utils.SegmentIntersectsRect(start, end, tileRect))
                            return true;
                    }
                }
            }
        }

        return false;
    }

    public virtual void AddAmmo(int ammo)
    {
        Ammo += ammo;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
