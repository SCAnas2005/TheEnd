
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;


public enum ZombieState
{
    Walk,
    Chase,
    Sleep
}

public class Zombie : Sprite, IDamageable, IHittable
{
    public (int, int) t;

    private int _blocks;
    private double _moveTimer = 0;
    private double _moveInterval = 100; // Temps en millisecondes entre chaque déplacement (ajuste selon la vitesse désirée)

    private int _directionCooldown = 0; // frames à attendre avant d’inverser à nouveau
    private const int CooldownDuration = 10; // par exemple, 10 frames
    private Rectangle _attackZone;
    private Rectangle _hitZone;

    private double _attackCooldown = 0;
    private const double AttackRate = 500; // ms
    private int _damage = 10;

    private const float PATH_UPDATE_INTERVAL = 1f; // toutes les 0.5 secondes

    private ZombieState _state;
    private Sprite _spriteToAttack;


    private Dictionary<string, List<AudioStreamPlayer>> _sounds;
    private float _nextSoundCooldown = 6000f;
    private float _soundTimer = 0f;

    public Zombie(Rectangle rect, string src, float speed, int health, Map map, bool debug = false) : base(rect, src, speed, health, map: map, debug: debug)
    {
        _speed = speed;

        _blocks = 0;
        _attackZone = Utils.AddToRect(_rect, new Rectangle(-50, -50, 100, 100));
        _hitZone = Utils.AddToRect(_rect, new Rectangle(-10, -10, 20, 20));

        zIndex = 2;
        _state = ZombieState.Walk;
        _spriteToAttack = null;

        _direction = Directions.right;

        _sounds = [];
    }

    public Zombie() : this(rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), src: "", speed: 2f, health: 50, map: null, debug: false)
    {
        
    }

    public override void Load(ContentManager Content)
    {
        CreateBaseAnimations();
        string name = "idle";
        animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Zombies/Idle/idle"), new Size(9, 15));

        name = "move_right";
        animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Zombies/WalkRight/walkright"), new Size(9, 15));

        name = "move_up";
        animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Zombies/WalkUp/walkup"), new Size(9, 15));

        name = "move_down";
        animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Zombies/WalkDown/walkdown"), new Size(9, 15));

        animation.Play("idle");


        _sounds["walk"] = [];
        for (int i = 0; i < 24; i++)
        {
            var s = $"sounds/sprites/zombies/walk-sounds/zombie-{i + 1}";
            var a = new AudioStreamPlayer2D(name: s, sound: Content.Load<SoundEffect>(s).CreateInstance(), entity: this)
            {
                MaxDistance = _attackZone.Width*5
            };
            a.SetListener(EntityManager.Player);
            _sounds["walk"].Add(a);
        }
    }   

    public void UpdateAttackZone()
    {
        _attackZone = Utils.AddToRect(_rect, new Rectangle(-50, -50, 100, 100));
    }


    public void UpdateHitZone()
    {
        _hitZone = Utils.AddToRect(_rect, new Rectangle(-10, -10, 20, 20));
    }


    public void OnHit(Sprite sprite)
    {
        sprite.TakeDamage(_damage);
    }


    public void AutomaticMove(GameTime gameTime)
    {
        var player = EntityManager.Player;
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Velocity = Direction * Speed * dt;


        switch (_state)
        {
            case ZombieState.Walk:
                if (Map == player.Map)
                {
                    _soundTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (_soundTimer >= _nextSoundCooldown)
                    {
                        var randomIndex = Utils.NextInt(0, _sounds["walk"].Count);
                        _sounds["walk"][randomIndex].Play();

                        _soundTimer = 0f;
                        _nextSoundCooldown = Utils.NextFloat(4, 10) * 1000f;
                        Console.WriteLine("Zombie playing sound");
                    }
                }
                if (_directionCooldown > 0)
                {
                    _directionCooldown--; // décrémente le timer de cooldown
                }

                // if ((player.Position - Position).Length() < 20)
                // {

                // }


                if (IsCollision && (IsCollisionLeft || IsCollisionRight) && _directionCooldown == 0)
                {
                    _direction.X = -_direction.X;
                    // vx = _dX * _speed;
                    _directionCooldown = CooldownDuration;
                    Velocity = Direction * Speed * dt;
                    SetColisionDirection(false);
                    IsCollision = false;
                }
                break;
            case ZombieState.Chase:
                if (_spriteToAttack != null && !_spriteToAttack.IsDead)
                {
                    if (_attackZone.Intersects(_spriteToAttack.Rect))
                    {
                        Vector2 direction = _spriteToAttack.Position - Position;
                        if (direction.LengthSquared() > 0.01f)
                        {
                            direction.Normalize();
                            _direction = new Vector2(Math.Sign(direction.X), Math.Sign(direction.Y));
                            Velocity = Direction * Speed * dt;
                        }
                    }
                    else
                    {
                        _state = ZombieState.Walk;
                        _spriteToAttack = null;
                    }
                }
                else
                {
                    _state = ZombieState.Walk;
                    _spriteToAttack = null;
                }
                break;
            default:
                break;
        }

        // if (_attackZone.Intersects(player.Rect))
        // {
        //     Vector2 direction = player.Position - Position;
        //     if (direction.LengthSquared() > 0.01f)
        //     {
        //         direction.Normalize();
        //         _dX = Math.Sign(direction.X);
        //         _dY = Math.Sign(direction.Y);

        //         vx = direction.X * _speed * dt;
        //         vy = direction.Y * _speed * dt;
        //     }
        // }



        // Animation
        if (Direction.X > 0)
        {
            if (animation.CurrentAnimationName != "move_right")
            {
                animation.Play("move_right");
            }
            animation.CurrentAnimation.HorizontalFlip = false;
        }

        else if (Direction.X < 0)
        {
            if (animation.CurrentAnimationName != "move_right")
            {
                animation.Play("move_right");
            }
            animation.CurrentAnimation.HorizontalFlip = true;
        }
        else if (Direction.Y > 0)
        {
            if (animation.CurrentAnimationName != "move_down")
            {
                animation.Play("move_down");
            }
        }
        else if (Direction.Y < 0)
        {
            if (animation.CurrentAnimationName != "move_up")
            {
                animation.Play("move_up");
            }
        }
        else
        {
            if (animation.CurrentAnimationName != "idle")
            {
                animation.Play("idle");
            }
        }

        TryMove(Map, Velocity);
    }


    public override void UpdateMovement(GameTime gameTime)
    {
        base.UpdateMovement(gameTime);
        if (CanMove)
        {
            AutomaticMove(gameTime);
        }
    }


    public override void Update(GameTime gameTime)
    {
        var player = EntityManager.Player;
        if (Health <= 0)
        {
            player.ZombieKilled++;
            _isDead = true;
        }
        List<Sprite> spritesToAttack = EntityManager.GetEntitiesWhere<Sprite>(e => e is Sprite s && s is not Zombie);

        foreach (var sprite in spritesToAttack)
        {
            if (!sprite.KillMe && !sprite.IsDead && _attackZone.Intersects(sprite.Rect))
            {
                _state = ZombieState.Chase;
                _spriteToAttack = sprite;
            }
        }

        UpdateAttackZone();
        UpdateHitZone();

        _attackCooldown = Math.Max(0, _attackCooldown - gameTime.ElapsedGameTime.TotalMilliseconds);
        if (_spriteToAttack != null && _hitZone.Intersects(_spriteToAttack.Rect) && _attackCooldown <= 0)
        {
            OnHit(_spriteToAttack);
            _attackCooldown = AttackRate;
        }

        UpdateMovement(gameTime);
        animation.Update(gameTime);
    }

    public override void UpdateOffscreen(GameTime gameTime)
    {
        base.UpdateOffscreen(gameTime);
        UpdateMovement(gameTime);
        animation.Update(gameTime);
    }


    public void DrawHealth(SpriteBatch _spriteBatch)
    {
        var a = Utils.AddToRect(_rect, new Rectangle(-3, -3, 6, -_rect.Height + 1));
        var h = a;
        h.Width = Utils.GetValueByPercentage(a.Width, Utils.GetPercentageByValue(MaxHealth, Health));
        Shape.FillRectangle(_spriteBatch, a, Color.Gray);
        Shape.FillRectangle(_spriteBatch, h, Color.Green);
    }


    public override void Draw(SpriteBatch _spriteBatch)
    {
        animation.Draw(_spriteBatch, _rect);
        DrawDebug(_spriteBatch);
        if (IsCollision)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Blue);
        }
        // Text.Write(_spriteBatch, $"x/y: {(_rect.X, _rect.Y)}, map:{_mapPos}", Vector2.Zero, Color.Blue);
        Shape.DrawRectangle(_spriteBatch, _attackZone, Color.Green);
        Shape.DrawRectangle(_spriteBatch, _hitZone, Color.LightBlue);
        DrawHealth(_spriteBatch);
    }
}