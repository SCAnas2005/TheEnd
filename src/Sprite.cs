
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

public abstract class Sprite
{
    protected Rectangle _rect;
    protected string _src;
    protected Texture2D _texture;

    protected int _health;

    protected int _speed;
    public bool debug;

    protected int _dX;
    protected int _dY;

    public int DirectionX {get{ return _dX; }}
    public int DirectionY {get{ return _dY; }}

    
    public int Health {get{ return _health; } set{ _health = value; }}


    public Map Map;

    public Rectangle Rect {get{return _rect;} set{_rect = value;}}
    public Size Size {get {return new Size(_rect.Width, _rect.Height);}}
    public Vector2 Position {get {return new Vector2(_rect.X, _rect.Y);} set{_rect.X = (int) value.X; _rect.Y =  (int)value.Y;}}
    public int Speed {get{return _speed;} set{_speed = value;}}
    public bool IsCollision;

    public bool IsCollisionLeft;
    public bool IsCollisionRight;
    public bool IsCollisionUp;
    public bool IsCollisionDown;     

    protected (int, int) _mapPos;

    public Sprite(Rectangle rect, string src, int speed, int health, bool debug = false)
    {
        _rect = rect;
        _speed = speed;
        _src = src;
        this.debug = debug;

        _health = health; 

        IsCollision = false;
        SetColisionDirection(); // All False

        _dX = 1;
        _dY = 1;
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

    public void SetColisionDirection(bool left=false, bool right=false, bool up=false, bool down=false)
    {
        IsCollisionLeft = left; IsCollisionRight = right; IsCollisionUp = up; IsCollisionDown = down;
    }


    public bool CheckCollision(Map map, Rectangle rect)
    {
        if (!map.TiledMap.Layers.Any(layer => layer.Name == "obstacles")) return false;

        if (rect.X < map.Rect.X || rect.X + rect.Width >= map.Rect.X + map.Rect.Width || rect.Y < map.Rect.Y || rect.Y + rect.Height >= map.Rect.Y + map.Rect.Height) return true;
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
                        // if (rect.X )
                        return IsCollision = true;
                    }
                }
            }
        }

        return IsCollision = false;
    }

    public void TryMove(Map map, int vx, int vy)
    {
        // Déplacement horizontal
        // Console.WriteLine("Rect collision: "+ CheckCollision(Map, _rect));
        int stepX = Math.Sign(vx);
        for (int i = 0; i < Math.Abs(vx); i++)
        {
            var testRect = Utils.AddToRect(_rect, new Rectangle(stepX, 0, 0, 0));
            if (!CheckCollision(map, testRect))
                _rect.X += stepX;
            else
            {
                if (vx > 0) SetColisionDirection(right:true, up: IsCollisionUp, down: IsCollisionDown);
                else if (vx < 0) SetColisionDirection(left:true, up: IsCollisionUp, down: IsCollisionDown);
                else { SetColisionDirection(); }
                break;
            }

        }

        // Déplacement vertical
        int stepY = Math.Sign(vy);
        for (int i = 0; i < Math.Abs(vy); i++)
        {
            var testRect = Utils.AddToRect(_rect, new Rectangle(0, stepY, 0, 0));
            if (!CheckCollision(map, testRect))
                _rect.Y += stepY;
            else
            {
                if (vy > 0) SetColisionDirection(down:true, right: IsCollisionRight, left:IsCollisionLeft);
                else if (vy < 0) SetColisionDirection(up:true, right: IsCollisionRight, left:IsCollisionLeft);
                else{ SetColisionDirection();}
                break;
            }
        }
    }


    public abstract void Load(ContentManager Content);

    public abstract void Update(GameTime gameTime);

    public virtual void Draw(SpriteBatch _spriteBatch)
    {
        if (debug)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Red);
        }
    }

}