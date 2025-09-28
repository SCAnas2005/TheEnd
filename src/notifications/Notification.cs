
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Notification
{
    public string Text { get; }
    public SpriteFont Font;
    public float TimeRemaining { get; private set; }

    private const float DisplayDuration = 3f;

    public Notification(string text, SpriteFont font=null)
    {
        Text = text;
        Font = font??CFonts.DefaultFont;
        TimeRemaining = DisplayDuration;
    }

    public void Update(GameTime gameTime)
    {
        TimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public bool IsExpired => TimeRemaining <= 0f;
}

