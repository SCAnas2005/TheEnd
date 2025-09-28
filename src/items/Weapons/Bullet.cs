
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// public class Bullet
// {
//     public Rectangle Rect;
//     public Vector2 Position { get { return new Vector2(Rect.X, Rect.Y); } }
//     public Vector2 PreviousPosition;
//     public int DirectionX;
//     public int speed;

//     public Bullet(Rectangle rect, int dx, int speed)
//     {
//         Rect = rect;
//         DirectionX = dx;
//         this.speed = speed;
//     }

//     public void Update(GameTime gameTime)
//     {
//         PreviousPosition = Position;
//         Rect.X += speed * DirectionX;
//     }

//     public void Draw(SpriteBatch _spriteBatch)
//     {
//         Shape.FillRectangle(_spriteBatch, Rect, Color.White);
//     }
// }


public class Bullet
{
    public Vector2 Position;
    public Vector2 PreviousPosition;
    public Vector2 Direction;
    public int speed;
    public int Size = 1;

    public Rectangle Rect => new Rectangle((int)Position.X, (int)Position.Y, Size, Size);

    public Bullet(Vector2 startPosition, Vector2 d, int speed)
    {
        Position = startPosition;
        PreviousPosition = Position;
        Direction = Vector2.Normalize(d);
        this.speed = speed;
    }

    public void Update(GameTime gameTime)
    {
        PreviousPosition = Position;
        Position += Direction * speed;
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        Shape.FillRectangle(_spriteBatch, Rect, Color.White);
    }
}