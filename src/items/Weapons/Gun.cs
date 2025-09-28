
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;

public class Gun(Rectangle rect, string name, Map map, IWeaponUser owner, int dx = 1, bool dropped = true, bool debug = false) : Weapon(rect, "Weapons/Guns/gun", name, map, owner, dx, dropped, debug)
{
    public Gun() : this(rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), name:"Gun", map: null, owner: null, dx:1, dropped:true, debug:false)
    {

    }
    public override void OnDrop()
    {
        base.OnDrop();
        Owner = null;
    }

    public override void Load(ContentManager Content)
    {
        base.Load(Content);
        _shotSound = Content.Load<SoundEffect>("sounds/weapons/gun");
        _impactOnMetalSound = Content.Load<SoundEffect>("sounds/entities/barrel/bullet-hit-metal");
        _selectWeaponSound = Content.Load<SoundEffect>("sounds/items/pickup-gun");
    }

    public override void Shoot()
    {
        if (Ammo > 0)
        {
            IsShooting = true;
            // Vector2 mousePos = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(Camera2D.GetViewMatrix()));
            // Vector2 direction = mousePos - new Vector2(_rect.X+_rect.Width, _rect.Y);
            _bullets.Add(new Bullet(
                startPosition: new Vector2(_rect.X+_rect.Width, _rect.Y),
                d: _direction,
                speed: 25
            ));
            Ammo--;
            AudioManager.Play(_shotSound, volume: 0.5f);
        }
    }

    



    public override void Update(GameTime gameTime)
    {
        Player player = EntityManager.Player;
        if (IsDropped)
        {
            base.Update(gameTime);
        }
        else
        {
            if (InputManager.GetMousePosition() != InputManager.GetPreviousMousePosition())
            {
                Vector2 mouseWorldPos = Vector2.Transform(InputManager.GetMousePosition(), Matrix.Invert(Camera2D.GetViewMatrix()));
                Vector2 weaponCenter = new Vector2(_rect.Center.X, _rect.Center.Y);
                _direction = mouseWorldPos - weaponCenter;
                _rotation = (float)Math.Atan2(_direction.Y, _direction.X);
            }

            IsShooting = InputManager.IsPressed(Keys.Space);
            if (IsShooting && player.CanUseItem)
            {
                Shoot();
            }

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                var bullet = _bullets[i];
                bullet.Update(gameTime);

                bool shouldRemove = false;
                Vector2 bulletStart = bullet.PreviousPosition;
                Vector2 bulletEnd = bullet.Position;

                // Vérifie collision avec la map
                if (CheckCollision(Map, bullet))
                {
                    shouldRemove = true;
                }
                else
                {
                    IDamageable closestTarget = null;
                    float closestDistance = float.MaxValue;

                    // Parcours toutes les entités
                    // foreach (var entity in ((GameScene)SceneManager.GetScene(SceneState.Game))._entities)
                    foreach (var entity in EntityManager.Entities)
                    {
                        // Ignore l'owner
                        if (entity == Owner)
                            continue;

                        if (entity is IDamageable dmg &&
                            Utils.SegmentIntersectsRect(bulletStart, bulletEnd, entity.Rect))
                        {
                            float dist = Vector2.Distance(bulletStart, entity.Rect.Center.ToVector2());
                            if (dist < closestDistance)
                            {
                                closestDistance = dist;
                                closestTarget = dmg;
                            }
                        }
                    }

                    if (closestTarget != null)
                    {
                        closestTarget.TakeDamage(_damage);

                        if (closestTarget is BarrelEntity)
                        {
                            AudioManager.Play(_impactOnMetalSound, volume: 0.2f);
                        }

                        shouldRemove = true;
                    }
                }

                if (shouldRemove)
                {
                    _bullets.RemoveAt(i);
                }
            }



        }
    }

    

    public override void Draw(SpriteBatch _spriteBatch)
    {
        // _spriteBatch.Draw(_texture, new Vector2(_rect.X, _rect.Y), null, Color.White, 0f, Vector2.Zero, 1f, DirectionX >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
        Vector2 position = new Vector2(_rect.Center.X, _rect.Center.Y);

        var effects = _rotation > MathF.PI / 2 || _rotation < -MathF.PI / 2
        ? SpriteEffects.FlipVertically
        : SpriteEffects.None;


        _spriteBatch.Draw(
            _texture,
            position,
            null,
            Color.White,
            _rotation,
            origin,
            1f,
            effects,
            0f
        );

        foreach (var bullet in _bullets)
        {
            bullet.Draw(_spriteBatch);
        }

        if (IsDropped)
        {
            DrawIndications(_spriteBatch);
            DrawZones(_spriteBatch);
        }

    }

}