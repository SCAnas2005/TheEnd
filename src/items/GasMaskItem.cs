
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
public class GasMaskItem : Item
{
    public bool _wearing = false;
    public bool IsWearing => !IsDropped && _wearing;
    public GasMaskItem(Rectangle rect, string name, Map map, bool dropped = true, bool debug = false) : base(rect, "Items/gas-mask", name, map, dropped, debug)
    {

    }

    public GasMaskItem() : this(new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), "GasMask", null)
    {

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (InputManager.IsPressed(Keys.J) && !IsDropped)
        {
            _wearing = true;
            Console.WriteLine("IsWearing: " + IsWearing);
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        var player = EntityManager.Player;
        base.Draw(_spriteBatch);
        if (IsWearing)
        {
            // Ratios dans le sprite original
            float rx = 2f / 14f;
            float ry = 0f / 16f;
            float rw = 10f / 14f;
            float rh = 5f / 16f;

            // Calculer le rectangle en coordonnées écran
            var offset = new Rectangle(
                player.Rect.X + (int)(player.Size.Width * rx),
                player.Rect.Y + (int)(player.Size.Height * ry),
                (int)(player.Size.Width * rw),
                (int)(player.Size.Height * rh)
            );
            var rect = Utils.AddToRect(offset);
            Shape.FillRectangle(_spriteBatch, rect, Color.Purple);
            Console.WriteLine("Drawing at rect : "+rect);
        }
    }
}