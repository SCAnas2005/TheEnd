
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

public enum ZombiesAnimation
{
    
}

public class Zombies : Sprite
{
    private Dictionary<Animation, AnimationSprite> _animations;
    public Animation CurrentAnimation;
    public (int, int) t;

    private int _blocks;
    private double _moveTimer = 0;
    private double _moveInterval = 100; // Temps en millisecondes entre chaque déplacement (ajuste selon la vitesse désirée)

    private int _directionCooldown = 0; // frames à attendre avant d’inverser à nouveau
    private const int CooldownDuration = 10; // par exemple, 10 frames
    public Player player;
    public MapScene MapScene;


    public Zombies(Rectangle rect, string src, int speed, int health, Map map, MapScene mapScene, bool debug = false) : base(rect, src, speed, health, debug)
    {
        _animations = [];
        CurrentAnimation = Animation.Idle;
        _speed = speed;

        _blocks = 0;
        Map = map;
        MapScene = mapScene;
    }

    public override void Load(ContentManager Content)
    { 
        AddAnimation(Animation.Idle, "Zombies/Idle/idle" , 1, 100, 9, 15, 1,1);
        AddAnimation(Animation.Right, "Zombies/WalkRight/walkright" , 3, 100, 9, 15, 1,3);
        AddAnimation(Animation.Down, "Zombies/WalkDown/walkdown" , 2, 100, 9, 15, 1,2);
        AddAnimation(Animation.Up, "Zombies/WalkUp/walkup" , 3, 100, 9, 15, 1,3);
        AddAnimation(Animation.Left, "Zombies/WalkLeft/walkleft" , 3, 100, 9, 15, 1,3);

    }

    public void AddAnimation(Animation a, string src, int frames, int frameTime, int width, int height, int LineNumber, int ColumnNumber, int LineIndex = 0, int ColumnIndex = 0, bool reverseFrame = false)
    {
        _animations[a] = new AnimationSprite(_rect, src, frames, frameTime, width, height, LineNumber, ColumnNumber, LineIndex, ColumnIndex, reverseFrame);
        _animations[a].Load(Globals.Content);
    }




    public void AutomaticMove()
    {
        int vx = _dX * _speed, vy = 0;

        if (_directionCooldown > 0)
        {
            _directionCooldown--; // décrémente le timer de cooldown
        }

        if (IsCollision && (IsCollisionLeft || IsCollisionRight) && _directionCooldown == 0)
        {
            Console.WriteLine($"Collision: left: {IsCollisionLeft} | right: {IsCollisionRight}");
            _dX = -_dX;
            // vx = _dX * _speed;
            _directionCooldown = CooldownDuration;
        }

        // Animation
        if (vy > 0) CurrentAnimation = Animation.Down;
        else if (vy < 0) CurrentAnimation = Animation.Up;
        else if (vx > 0) CurrentAnimation = Animation.Right;
        else if (vx < 0) CurrentAnimation = Animation.Left;
        else CurrentAnimation = Animation.Idle;

        TryMove(Map, vx, vy);
    }




    public override void Update(GameTime gameTime)
    {
        _moveTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (_moveTimer >= _moveInterval)
        {
            AutomaticMove();
            _moveTimer = 0;
        }
        _animations[CurrentAnimation].Update(gameTime);
    }


    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (_animations.Count > 0)
        {
            _animations[CurrentAnimation].Draw(_spriteBatch, _rect);
        }
        base.Draw(_spriteBatch);
        if (IsCollision)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Blue);
        }
        // Text.Write(_spriteBatch, $"x/y: {(_rect.X, _rect.Y)}, map:{_mapPos}", Vector2.Zero, Color.Blue);
    }
}