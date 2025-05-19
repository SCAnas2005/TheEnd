
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

public class Weapon : Item
{
    private int _dX;
    public int DirectionX {get{ return _dX; } set{ _dX = value; }}

    public bool IsShooting = false;

    public int ammo;
    private List<Bullet> _bullets;


    private bool IsAmoCollision;

    public Weapon(Rectangle rect, string src, string name, Map map, MapScene mapScene, int dx = 1) : base(rect, src, name, map, mapScene)
    {
        _dX = dx;
        ammo = 5;
        _bullets = [];
    }

    public override string GetConditionName() => "Prendre";
    public override string GetConditionInstruction() => $"Appuyer sur [E] pour [{GetConditionName()}]";


    public void Shoot()
    {
        IsShooting = true;
        if (ammo > 0)
        {
            _bullets.Add(new Bullet(
                rect: new Rectangle(_rect.X + _rect.Width, _rect.Y, 3, 1),
                dx: _dX,
                speed: 8
            ));

            ammo--;
        }

    }

    
    public bool CheckCollision(Map map, Rectangle rect)
    {
        if (!map.TiledMap.Layers.Any(layer => layer.Name == "obstacles")) return false;

        if (rect.X < 0 || rect.X + rect.Width >= map.Rect.X + map.Rect.Width || rect.Y < 0 || rect.Y + rect.Height >= map.Rect.Y + map.Rect.Height) return true;
        var collisionLayer = map.TiledMap.GetLayer<TiledMapTileLayer>("obstacles");

        // Calculer la position de la tuile en ligne et colonne pour chaque coin du personnage
        (int startRow, int startCol) = Map.GetMapPosFromVector2(new Vector2(rect.X, rect.Y), map.TileSize);

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
                        // if (rect.X )
                        return IsAmoCollision = true;
                    }
                }
            }
        }

        return IsAmoCollision = false;
    }

    // public void TryMove(Map map, ref Rectangle rect, int vx, int vy)
    // {
    //     // Déplacement horizontal
    //     // Console.WriteLine("Rect collision: "+ CheckCollision(Map, _rect));
    //     int stepX = Math.Sign(vx);
    //     for (int i = 0; i < Math.Abs(vx); i++)
    //     {
    //         var testRect = Utils.AddToRect(rect, new Rectangle(stepX, 0, 0, 0));
    //         if (!CheckCollision(map, testRect))
    //             rect.X += stepX;
    //         else
    //         {
    //             break;
    //         }

    //     }

    //     // Déplacement vertical
    //     int stepY = Math.Sign(vy);
    //     for (int i = 0; i < Math.Abs(vy); i++)
    //     {
    //         var testRect = Utils.AddToRect(rect, new Rectangle(0, stepY, 0, 0));
    //         if (!CheckCollision(map, testRect))
    //             rect.Y += stepY;
    //         else
    //         {
    //             break;
    //         }
    //     }
    // }

    public override void Action(Player player, Map map)
    {
        player.gun = this;
        IsDropped = false;
    }

    public override void Update(GameTime gameTime, Player player, Map map)
    {
        if (IsDropped)
        {
            base.Update(gameTime, player, map);
        }
        else
        {
            IsShooting = InputManager.IsPressed(Microsoft.Xna.Framework.Input.Keys.Space);
            if (IsShooting)
            {
                Shoot();
            }

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);
                if (CheckCollision(Map, _bullets[i].Rect))
                {
                    _bullets.RemoveAt(i);
                }
            }
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.Draw(_texture, new Vector2(_rect.X, _rect.Y), null, Color.White, 0f, Vector2.Zero, 1f, DirectionX >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        foreach (var bullet in _bullets)
        {
            bullet.Draw(_spriteBatch);
        }

        DrawIndications(_spriteBatch);
    }

}