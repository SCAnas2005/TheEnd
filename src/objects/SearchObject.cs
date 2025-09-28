
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class SearchObject : InteractionObject
{
    public Item ItemToGive;
    public SearchObject(
        Rectangle rect,
        MapScene mapScene,
        string name = "searchbox",
        Item itemToGive = null,
        Func<string> actionName = null,
        Func<string> actionInstructions = null, Keys key = Keys.E) : base(rect, type:"searchbox", mapScene, name, l:null, c:null, actionName, actionInstructions, key)
    {
        ItemToGive = itemToGive;
        EditAction(
            "interact",
            name: "Fouiller",
            action: (player) =>
            {
                if (itemToGive == null) return;
                if (!player.Inventory.IsFull)
                {
                    itemToGive.IsDropped = false;
                    player.Inventory.AddItem(itemToGive);
                }
                else
                {
                    itemToGive.IsDropped = true;
                    itemToGive.Position = player.Position;
                }

                NotificationManager.Add(new Notification(text: "Vous avez trouve un nouvel item", font: CFonts.Minecraft_24));
                Destroy();
            }
        );
    }
}