
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public class ArmoireObject : InteractionObject
{
    public bool IsIntersectWithPlayer {get; set;}
    public Item Item;
    public MapScene? destinationMap;
    public ArmoireObject(
        Rectangle rect,
        string type,
        string name,
        MapScene mapScene,
        int? l, int? c,
        Item item,
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, mapScene, name, l, c, actionName, actionInstructions)
    {
        IsIntersectWithPlayer = false;
        Item = item;
    }

    public override string GetConditionName() => ActionName == null ? "[Fouiller]" : ActionName?.Invoke();
    public override string GetConditionInstruction() => ActionInstructions == null ? $"Appuyer sur [E] pour {GetConditionName()}" : ActionInstructions?.Invoke();

    public override bool IsConditionDone(Map map, Player player)
    {
        IsIntersectWithPlayer = rect.Intersects(player.Rect);
        if (InputManager.IsPressed(Keys.E) && IsIntersectWithPlayer)
        {
            return true;
        }
        return false;
    }

    public override void DoAction(Map map, Player player)
    {
        if (Item != null)
        {
            Item.IsDropped = false;
            player.Inventory.AddItem(Item);
            Console.WriteLine("You found a new item in your inventory!");
            Item = null; // Remove the item from the armoire
        }
    }

    public override void Update(GameTime gametime, Map map, Player player){
        if (IsConditionDone(map, player))
        {
            DoAction(map, player);
        }
    }

    public override void Draw(SpriteBatch _spriteBatch) {
        if (IsIntersectWithPlayer)
        {
            Size s = Text.GetSize(GetConditionName(), scale:0.3f);
            Vector2 p = new Vector2(rect.X+rect.Width, rect.Y+(rect.Height-s.Height)/2);
            Text.Write(_spriteBatch, GetConditionName(), p, Color.Blue, scale: 0.3f);
            p.Y+=s.Height;
            Text.Write(_spriteBatch, GetConditionInstruction(), p, Color.Blue, scale:0.3f);
        }
    }

}