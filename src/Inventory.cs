

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Inventory
{

    
    public Item[] Items;
    public Item First { get { return Items[0]; } }
    public Item Last { get { return Items[Storage-1]; } }
    public int Storage;
    public int ItemNumber;
    public Item SelectedItem {get{ return Items[SelectedItemIndex]; }}

    public bool IsFull {get{ return ItemNumber >= Storage; }}
    public bool IsEmpty {get{ return ItemNumber == 0; }}

    public int SelectedItemIndex;


    public Inventory(int storageMax)
    {
        Storage = storageMax;
        Items = new Item[Storage];
        ItemNumber = 0;
        SelectedItemIndex = 0;
    }

    public Item[] GetItems()
    {
        List<Item> l = [];
        foreach (var item in Items)
        {
            if (item != null)
                l.Add(item);
        }
        return l.ToArray();
    }


    public void AddItem(Item newItem)
    {
        if (IsFull) return;
        for (int i = 0; i < Storage; i++)
        {
            if (Items[i] == null)
            {
                Items[i] = newItem;
                break;
            }
        }
        ItemNumber++;
    }

    public void RemoveItem(int i)
    {
        if (!IsEmpty && Items[i] != null)
        {
            Items[i].IsDropped = true;
            Console.WriteLine($"Removed item at index : {i} : {Items[i]}");
            Items[i] = null;
            ItemNumber--;
        }
    }

    public void UpdateSelectedItem(GameTime gameTime, Player player, Map map)
    {   
        if (SelectedItemIndex >= 0 && SelectedItemIndex < ItemNumber)
        {
            Items[SelectedItemIndex]?.Update(gameTime, player, map);
        }
    }

    public void DrawSelectedItem(SpriteBatch _spriteBatch)
    {
        if (SelectedItemIndex >= 0 && SelectedItemIndex < ItemNumber)
        {
            Items[SelectedItemIndex]?.Draw(_spriteBatch);
        }
    }

    public void RemoveSelectedItem()
    {
        if (Items[SelectedItemIndex] != null)
        {
            RemoveItem(SelectedItemIndex);
        }
    }

    public void PrintInventory()
    {
        foreach (var item in Items)
        {
            Console.WriteLine($"{item} - ");
        }
    }

    public override string ToString()
    {
        return $"[INVENTORY] Max item number: {Storage}, current item number: {ItemNumber}";
    }
}   