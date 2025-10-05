


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using TheEnd;

public class Player : Sprite, IDamageable, IWeaponUser
{
    public Inventory Inventory;
    public int ZombieKilled;

    public List<Zombie> zombies;

    public bool IsTakingDamage = false;
    private float DamageAnimationRate = 500; // 500ms
    private float DamageAnimationCoolDown = 0;

    public int Money = 0;

    private SoundEffect _footstepGrass;
    private double _footstepTimer = 0;
    private double _footstepInterval = 500; // en ms, adapte selon la vitesse du joueurs

    public bool CanUseItem = true;
    public bool CanSelectItem = true;
    public bool CanInteract = true;

    public Player(
        Rectangle rect,
        string src, string name,
        float speed, Map map, int health,
        Inventory inventory = null,
        int money = 0,
        bool debug = false
    ) : base(rect, src, speed, health, map: map, name: name, debug: debug)
    {
        animation = new AnimatedSprite2D();
        _speed = 200f;

        ZombieKilled = 0;

        Inventory = inventory ?? new Inventory(5);
        Money = money;

        zIndex = 5;
    }

    public void SetNewMap(Map newMap)
    {
        Map = newMap;
        foreach (var item in Inventory.GetItems())
        {
            item.Map = Map;
        }
    }

    

    public void AddMoney(int amount)
    {
        Money += amount;
    }

    public void UpdateGunPosition()
    {
        foreach (var item in Inventory.GetItems())
        {
            if (item is Weapon)
            {
                item.Rect = new Rectangle(
                    x: _rect.X + Utils.GetValueByPercentage(_rect.Width, -7),
                    y: _rect.Y + Utils.GetValueByPercentage(_rect.Height, 38),
                    width: Utils.GetValueByPercentage(_rect.Width, 114),
                    height: Utils.GetValueByPercentage(_rect.Height, 31)
                );
            }
        }
    }

    public override void Load(ContentManager Content)
    {
        _texture = Content.Load<Texture2D>("Player/pp");


        CreateBaseAnimations();
        string name = "idle";
        animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Player/Idle/idle"), new Size(14, 16));

        name = "move_right";
        animation.AddFrame(name, "Player/WalkRight/right1");
        animation.AddFrame(name, "Player/WalkRight/right2");
        animation.AddFrame(name, "Player/WalkRight/right3");
        // animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Player/WalkRight/walkright"), new Size(14, 15));

        name = "move_up";
        animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Player/WalkUp/walkup"), new Size(14, 16));

        name = "move_down";
        animation.FromSpritesheet(name, Globals.Content.Load<Texture2D>("Player/WalkDown/walkdown"), new Size(14, 15));

        name = "takedamage_idle";
        animation.AddFrame(name, "Player/Damage/idle");

        name = "takedamage_right";
        animation.AddFrame(name, "Player/Damage/right");

        name = "takedamage_up";
        animation.AddFrame(name, "Player/Damage/up");


        foreach (var item in Inventory.GetItems())
        {
            item?.Load(Content);
        }

        _footstepGrass = Content.Load<SoundEffect>("sounds/sprites/human/footstep_grass");
    }

    public void AddHealth(int newHealth)
    {
        Health += newHealth;
    }

    public Item DropItem()
    {
        return Inventory.RemoveSelectedItem();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        IsTakingDamage = true;
        DamageAnimationCoolDown = DamageAnimationRate;
    }


    public void UpdateDamageAnimation(GameTime gameTime)
    {
        if (DamageAnimationCoolDown > 0)
        {
            DamageAnimationCoolDown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        else
        {
            DamageAnimationCoolDown = DamageAnimationRate;
            IsTakingDamage = false;
        }
    }




    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Réinitialise les directions
        _direction = Vector2.Zero;

        // Gestion verticale
        if (CanMove)
        {
            if (InputManager.IsHolding(Keys.S))
            {
                _direction += Directions.down;
            }
            else if (InputManager.IsHolding(Keys.Z))
            {
                _direction += Directions.up;
            }
            // Gestion horizontale
            if (InputManager.IsHolding(Keys.D))
            {
                _direction += Directions.right;
                if (Inventory.SelectedItem != null && Inventory.SelectedItem is Gun)
                {
                    ((Gun)Inventory.SelectedItem).Direction = Directions.right;
                }
            }
            else if (InputManager.IsHolding(Keys.Q))
            {
                _direction += Directions.left;
                if (Inventory.SelectedItem != null && Inventory.SelectedItem is Gun)
                {
                    ((Gun)Inventory.SelectedItem).Direction = Directions.left;
                }
            }

            _footstepTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if ((_direction.X != 0 || _direction.Y != 0) && _footstepTimer <= 0f)
            {
                if (Map.IsInGrass(Position)) // ← ta fonction déjà codée
                {
                    AudioManager.Play(_footstepGrass, volume: 0.2f); // ou .Play("grass") si tu l’as enregistré par string
                    _footstepTimer = _footstepInterval;
                }
            }

        }

        // Détermine l'animation en fonction de la direction prioritaire
        if (_direction == Vector2.Zero)
        { 
            if (!IsTakingDamage)
            {
                if (animation.CurrentAnimationName != "idle")
                    animation.Play("idle");
            }
            else
            {
                if (animation.CurrentAnimationName != "takedamage_idle")
                    animation.Play("takedamage_idle");
            }
        }else if (_direction.X != 0)
        {
            if (!IsTakingDamage)
            {
                if (animation.CurrentAnimationName != "move_right")
                    animation.Play("move_right");
            }
            else
            {
                if (animation.CurrentAnimationName != "takedamage_right")
                    animation.Play("takedamage_right");
            }

            // Ici tu mets juste le flip
            animation.CurrentAnimation.HorizontalFlip = (_direction.X < 0);
        }
        else if (_direction.Y != 0)
        {
            if (_direction.Y > 0)
            {
                if (IsTakingDamage)
                {
                    if (animation.CurrentAnimationName != "takedamage_idle")
                        animation.Play("takedamage_idle");
                }
                else
                {
                    if (animation.CurrentAnimationName != "move_down")
                        animation.Play("move_down");
                }
            }
            else // _direction.Y < 0
            {
                if (!IsTakingDamage)
                {
                    if (animation.CurrentAnimationName != "move_up")
                        animation.Play("move_up");
                }
                else
                {
                    if (animation.CurrentAnimationName != "takedamage_up")
                        animation.Play("takedamage_up");
                }
            }
        }


        Velocity = Direction * Speed * dt;
        TryMove(Map, Velocity);
        UpdateGunPosition();

        animation.Update(gameTime);
        Inventory.UpdateSelectedItem(gameTime);
        if (IsTakingDamage)
        {
            UpdateDamageAnimation(gameTime);
        }

        if (CanMove && CanSelectItem)
        {
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
                        Inventory.SetIndex(i - 1);
                    }
                    break;
                }
            }
        }

        if (InputManager.IsPressed(Keys.I))
        {
            Inventory.PrintInventory();
        }
    }

    public void DrawPosition(SpriteBatch _spriteBatch)
    {
        Text.Write(_spriteBatch, $"x/y: {(_rect.X, _rect.Y)}, map:{_mapPos}", Vector2.Zero, Color.Blue);
    }


    public override void Draw(SpriteBatch _spriteBatch)
    {
        animation.Draw(_spriteBatch, Rect);

        DrawDebug(_spriteBatch);
        if (IsCollision)
        {
            Shape.DrawRectangle(_spriteBatch, _rect, Color.Blue);
        }
        Inventory.DrawSelectedItem(_spriteBatch);
    }

    
}