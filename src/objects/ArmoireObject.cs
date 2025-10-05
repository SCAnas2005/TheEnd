
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public class ArmoireObject : InteractionObject
{
    public Item Item;
    public MapScene? destinationMap;
    public ArmoireObject(
        Rectangle rect,
        string type,
        string name,
        Map map,
        int? l, int? c,
        Item item,
        Func<string> actionName = null, Func<string> actionInstructions = null
    ) : base(rect, type, map, name, l, c, actionName, actionInstructions)
    {
        IsIntersectWithPlayer = false;
        Item = item;



        EditAction(
            keyName: "interact",
            name: "Fouiller",
            action: (player) =>
            {
                if (Item != null)
                {
                    Console.WriteLine("Fouiller");
                    Item.IsDropped = false;
                    player.Inventory.AddItem(Item);
                    Item = null; // Remove the item from the armoire
                }
            }
        );
    }

    public void AddItem(Item item)
    {
        item.IsDropped = false;
        Item = item;
    }


    public override void Update(GameTime gametime, Map map, Player player)
    {
        base.Update(gametime, map, player);
    }
}