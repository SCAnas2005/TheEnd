
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Bullet
{
    public Rectangle Rect;
    public int DirectionX;
    public int speed;

    public Bullet(Rectangle rect, int dx, int speed)
    {
        this.Rect = rect;
        DirectionX = dx;
        this.speed = speed;
    }

    public void Update(GameTime gameTime)
    {
        Rect.X += speed * DirectionX;
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        Shape.FillRectangle(_spriteBatch, Rect, Color.White);
    }
}