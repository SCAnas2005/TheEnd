

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class MedkitItem(Rectangle rect, string name, Map map, int health = 100, bool dropped = true, bool debug = false) : HealItem(rect, "Items/medkit-green", name, map, health, dropped, debug)
{
    public MedkitItem() : this(new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()), "Medkit", null)
    {
        
    }
}