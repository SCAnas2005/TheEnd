


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using TheEnd;

public enum Animation {
    Idle,
    Right,
    Left, 
    Up,
    Down
}

public class Player : Sprite
{
    private Dictionary<Animation, AnimationSprite> _animations;
    public Animation CurrentAnimation;

    public Inventory Inventory;

    public Player(Rectangle rect, string src, int speed, Map map, bool debug = false) : base(rect, src, speed, debug)
    {
        _animations = [];
        CurrentAnimation = Animation.Idle;
        _speed = 3;
        Map = map;

        Inventory = new Inventory(5);
    }

    public void SetNewMap(Map newMap)
    {
        Map = newMap;
        foreach (var item in Inventory.GetItems())
        {
            item.Map = Map;
            item.MapScene = Map.Scene;
        }
    }

    public void UpdateGunPosition()
    {
        foreach (var item in Inventory.GetItems())
        {
            if (item is Weapon)
            {
                item.Rect = new Rectangle(
                    x: _rect.X + Utils.GetPercentageInt(_rect.Width, -7),
                    y: _rect.Y + Utils.GetPercentageInt(_rect.Height, 38),
                    width: Utils.GetPercentageInt(_rect.Width, 114),
                    height: Utils.GetPercentageInt(_rect.Height, 31)
                );
            }
        }
    }

    public override void Load(ContentManager Content)
    {
        _texture = Content.Load<Texture2D>(_src);

        AddAnimation(Animation.Idle, "Player/Idle/idle", 1, 100, 14, 15, 1, 1);
        AddAnimation(Animation.Right, "Player/WalkRight/walkright", 3, 100, 14, 15, 1, 3);
        AddAnimation(Animation.Down, "Player/WalkDown/walkdown", 2, 100, 14, 15, 1, 2);
        AddAnimation(Animation.Up, "Player/WalkUp/walkup", 3, 100, 14, 15, 1, 3);
        AddAnimation(Animation.Left, "Player/WalkLeft/walkleft", 3, 100, 14, 15, 1, 3);

        foreach (var item in Inventory.GetItems())
        {
            item?.Load(Content);
        }
    }

    public void AddAnimation(Animation a, string src, int frames, int frameTime, int width, int height, int LineNumber, int ColumnNumber, int LineIndex = 0, int ColumnIndex = 0, bool reverseFrame = false)
    {
        _animations[a] = new AnimationSprite(_rect, src, frames, frameTime, width, height, LineNumber, ColumnNumber, LineIndex, ColumnIndex, reverseFrame);
        _animations[a].Load(Globals.Content);
    }

    public void DropItem()
    {
        Inventory.RemoveSelectedItem();
    }


    public override void Update(GameTime gameTime)
    {
        // Réinitialise les directions
        _dX = 0;
        _dY = 0;

        // Gestion verticale
        if (InputManager.IsHolding(Keys.S))
        {
            _dY = 1;
        }
        else if (InputManager.IsHolding(Keys.Z))
        {
            _dY = -1;
        }

        // Gestion horizontale
        if (InputManager.IsHolding(Keys.D))
        {
            _dX = 1;
            if (Inventory.SelectedItem != null && Inventory.SelectedItem is Weapon)
            {
                ((Weapon)Inventory.SelectedItem).DirectionX = _dX;
            }
        }
        else if (InputManager.IsHolding(Keys.Q))
        {
            _dX = -1;
            if (Inventory.SelectedItem != null && Inventory.SelectedItem is Weapon)
            {
                ((Weapon)Inventory.SelectedItem).DirectionX = _dX;
            }
        }

        // Détermine l'animation en fonction de la direction prioritaire
        if (_dX == 0 && _dY == 0)
        {
            CurrentAnimation = Animation.Idle;
        }
        else if (_dX != 0)
        {
            CurrentAnimation = (_dX > 0) ? Animation.Right : Animation.Left;
        }
        else if (_dY != 0)
        {
            CurrentAnimation = (_dY > 0) ? Animation.Down : Animation.Up;
        }

        int vx = _dX * _speed;
        int vy = _dY * _speed;

        TryMove(Map, vx, vy);
        UpdateGunPosition();

        _animations[CurrentAnimation].Update(gameTime);
        Inventory.UpdateSelectedItem(gameTime, this, Map);

        if (InputManager.IsPressed(Keys.L))
        {
            DropItem();
        }

        for (int i = 1; i <= 5; i++)
        {
            Keys key = (Keys)((int)Keys.NumPad0 + i);
            Keys key2 = (Keys)((int)Keys.D0 + i);
            if (InputManager.IsPressed(key) || InputManager.IsPressed(key2))
            {
                if (Inventory.SelectedItemIndex != i - 1)
                {
                    Inventory.SelectedItemIndex = i - 1;
                    Console.WriteLine($"New item selected {i} : {(Inventory.SelectedItem != null ? Inventory.SelectedItem.Name : null)}");
                }
                break;
            }
        }

    }


    public override void Draw(SpriteBatch _spriteBatch){
        _animations[CurrentAnimation].Draw(_spriteBatch, _rect);
        base.Draw(_spriteBatch);
        if (IsCollision)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Blue);  
        }
        Inventory.DrawSelectedItem(_spriteBatch);
        Text.Write(_spriteBatch, $"x/y: {(_rect.X, _rect.Y)}, map:{_mapPos}", Vector2.Zero, Color.Blue);
    }
}