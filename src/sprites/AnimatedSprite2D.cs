

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AnimatedSprite2D
{
    private Dictionary<string, AnimatedSprite> _animtions;
    private double _timer;

    public string CurrentAnimationName = null;
    public AnimatedSprite CurrentAnimation { get { return CurrentAnimationName != null ? _animtions[CurrentAnimationName] : null; } }
    public Texture2D CurrentAnimationFrame { get { return CurrentAnimationName != null ? CurrentAnimation.CurrentFrame : null; } }

    public AnimatedSprite2D()
    {
        _animtions = new();
    }

    public AnimatedSprite Get(string name)
    {
        if (_animtions.ContainsKey(name)) return _animtions[name]; return null;
    }

    public AnimatedSprite AddAnimation(string name = "default")
    {
        var a = new AnimatedSprite();
        _animtions[name] = a;
        return a;
    }

    public void AddFrame(string name, Texture2D texture)
    {
        if (!_animtions.ContainsKey(name))
        {
            AddAnimation(name);
        }
        _animtions[name].AddFrame(texture);
    }
    public void AddFrame(string name, string texturePath)
    {
        var texture = Globals.Content.Load<Texture2D>(texturePath);
        AddFrame(name, texture);
    }

    public void FromSpritesheet(string name, Texture2D spritesheet, Size frameSize, Rectangle? offset = null)
    {
        int cols = spritesheet.Width / frameSize.Width;
        int rows = spritesheet.Height / frameSize.Height;

        Rectangle offs = offset ?? Rectangle.Empty;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                // Rectangle de la cellule complète (ex: 200x200)
                var cell = new Rectangle(
                    x * frameSize.Width,
                    y * frameSize.Height,
                    frameSize.Width,
                    frameSize.Height
                );

                // Zone utile à l’intérieur de la cellule
                var rectSource = new Rectangle(
                    cell.X + offs.X,
                    cell.Y + offs.Y,
                    frameSize.Width + offs.Width,
                    frameSize.Height + offs.Height
                );

                // sécurité : pas dépasser la texture
                if (rectSource.Right > spritesheet.Width || rectSource.Bottom > spritesheet.Height)
                    continue;

                // Crée une texture frame de la taille utile seulement
                Texture2D frame = new Texture2D(spritesheet.GraphicsDevice, rectSource.Width, rectSource.Height);
                Color[] data = new Color[rectSource.Width * rectSource.Height];

                spritesheet.GetData(0, rectSource, data, 0, data.Length);
                frame.SetData(data);

                _animtions[name].AddFrame(frame);
            }
        }
    }


    public void Update(GameTime gameTime)
    {
        if (CurrentAnimationName == null) return;
        _timer += gameTime.ElapsedGameTime.TotalMilliseconds;
        if (_timer >= CurrentAnimation.FrameDuration)
        {
            _timer = 0;
            CurrentAnimation.CurrentIndex++;
            if (CurrentAnimation.CurrentIndex >= CurrentAnimation.Count)
            {
                if (CurrentAnimation.Looping)
                    CurrentAnimation.CurrentIndex = 0;
                else
                {
                    CurrentAnimation.CurrentIndex = CurrentAnimation.Count - 1;
                    CurrentAnimation.IsFinished = true;
                }
            }
        }

    }

    public AnimatedSprite Play(string name)
    {
        if (_animtions.ContainsKey(name))
        {
            CurrentAnimationName = name;
            CurrentAnimation.CurrentIndex = 0;
            _timer = 0;
            CurrentAnimation.IsFinished = false;
            return CurrentAnimation;
        }
        return null;
    }

    public void Draw(SpriteBatch _spriteBatch, Rectangle rect)
    {
        if (CurrentAnimationName == null || CurrentAnimationFrame == null) return;
        var effects = CurrentAnimation.HorizontalFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (CurrentAnimation.VerticalFlip) effects |= SpriteEffects.FlipVertically;
        _spriteBatch.Draw(
            texture: CurrentAnimationFrame,
            destinationRectangle: rect, sourceRectangle: null,
            color: Color.White,
            rotation: CurrentAnimation.Rotation, origin: CurrentAnimation.Origin,
            effects: effects,
            layerDepth: 0f
        );
    }

    public void Draw(SpriteBatch _spriteBatch, Vector2 position)
    {
        if (CurrentAnimationName == null || CurrentAnimationFrame == null) return;

        var effects = CurrentAnimation.HorizontalFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (CurrentAnimation.VerticalFlip) effects |= SpriteEffects.FlipVertically;

        _spriteBatch.Draw(
            texture: CurrentAnimationFrame,
            position: position,
            sourceRectangle: null,
            color: Color.White,
            rotation: CurrentAnimation.Rotation,
            origin: CurrentAnimation.Origin,
            scale: CurrentAnimation.Scale, // ← ici on utilise le scale de l'animation
            effects: effects,
            layerDepth: 0f
        );
    }

}


public class AnimatedSprite
{
    private List<Texture2D> _textures;
    public int CurrentIndex;
    public Texture2D CurrentFrame { get { return !IsEmpty ? _textures[CurrentIndex] : null; } }

    private int _framePerSeconds;
    public int FramePerSeconds { get => _framePerSeconds; set { _framePerSeconds = value; FrameDuration = 1000f / FramePerSeconds; } }
    public float FrameDuration { get; set; }

    public int Count { get => _textures.Count; }
    public bool IsEmpty { get => Count == 0; }

    public bool Looping = true;

    public bool HorizontalFlip = false;
    public bool VerticalFlip = false;

    public float Rotation;
    public Vector2 Origin;

    public Vector2 Scale;

    private bool _finished = false;
    public bool IsFinished {get { return _finished; } set{ _finished = value; }}

    public AnimatedSprite(int framePerSeconds = 5)
    {
        _textures = [];
        CurrentIndex = 0;
        FramePerSeconds = framePerSeconds;
        Rotation = 0f;
        Origin = Vector2.Zero;

        Scale = Vector2.One;
    }

    public void AddFrame(Texture2D texture)
    {
        _textures.Add(texture);
    }

    public void Insert(Texture2D texture, int index)
    {
        _textures.Insert(index, texture);
    }

    public void RemoveAt(int index)
    {
        _textures.RemoveAt(index);
    }

}