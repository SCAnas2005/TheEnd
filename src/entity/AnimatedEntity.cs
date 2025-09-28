
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public abstract class AnimatedEntity : Entity
{
    protected AnimatedSprite2D _animation;

    public AnimatedEntity(Rectangle rect, string src, Map map) : base(rect, src, map)
    {
        _animation = new AnimatedSprite2D();
    }

    public override void Load(ContentManager Content)
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        _animation.Update(gameTime);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _animation.Draw(_spriteBatch, Rect);
    }
}
