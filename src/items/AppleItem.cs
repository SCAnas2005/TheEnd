    using Microsoft.Xna.Framework;

public class AppleItem : HealItem
{
    public AppleItem(Rectangle rect, string name, Map map, int health = 10, bool dropped = true, bool debug = false) : base(rect, "Items/apple-red", name, map, health, dropped, debug: debug) { }

    public AppleItem() : this(new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), "Apple", null)
    {

    }
}