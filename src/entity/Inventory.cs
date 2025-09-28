

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

    private AudioStreamPlayer itemEquip;


    public Inventory(int storageMax, Item[] items = null)
    {
        Storage = storageMax;
        Items = items ?? new Item[Storage];
        ItemNumber = 0;
        SelectedItemIndex = 0;

        itemEquip = new AudioStreamPlayer("item_equip", "sounds/items/item-equip");
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

    public bool Has(Type type)
    {
        foreach (var item in Items)
        {
            if (type.IsInstanceOfType(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool Has(Item i)
    {
        foreach (var item in Items)
        {
            if (item == i)
            {
                return true;
            }
        }
        return false;
    }

    public T GetFirst<T>() where T : class
    {
        foreach (var item in Items)
        {
            if (item is T tItem)
            {
                return tItem;
            }
        }
        return null;
    }

    public List<T> GetAll<T>() where T : class
    {
        List<T> l = [];
        foreach (var item in Items)
        {
            if (item is T tItem)
            {
                l.Add(tItem);
            }
        }
        return l;
    }

    public void SetIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < Storage && newIndex != SelectedItemIndex)
        {
            SelectedItem?.OnChange();
            SelectedItemIndex = newIndex;
            SelectedItem?.OnSelect();

            if (SelectedItem != null)
            {
                itemEquip.Play();
            }
        }
    }


    public void AddItem(Item newItem)
    {
        if (!IsFull)
        {
            for (int i = 0; i < Storage; i++)
            {
                if (Items[i] == null)
                {
                    if (newItem is Weapon w)
                    {
                        var ammos = GetAll<AmmoItem>();
                        if (ammos.Count > 0)
                        {
                            foreach (var a in ammos)
                            {
                                w.AddAmmo(a.Ammo);
                                RemoveItem(a);
                                a.Kill();
                            }
                        }
                    }
                    else if (newItem is AmmoItem ammo)
                    {
                        var weapon = GetFirst<Weapon>();
                        if (weapon != null) weapon.AddAmmo(ammo.Ammo);
                    }
                    newItem.OnPickUp();
                    Items[i] = newItem;
                    if (i == SelectedItemIndex)
                    {
                        SelectedItem?.OnSelect();
                    }
                    break;
                }
            }
            ItemNumber++;
        }
        else
        {
            SelectedItem.OnDrop();
            Items[SelectedItemIndex] = newItem;
        }
    }

    public void RemoveItem(Item item)
    {
        if (!IsEmpty && item != null)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == item)
                {
                    RemoveItemAt(i);
                    return;
                }
            }
        }
    }

    public Item RemoveItemAt(int i)
    {
        Item itemToRemove = null;
        if (!IsEmpty && Items[i] != null)
        {
            itemToRemove = Items[i];
            itemToRemove.OnDrop();
            Items[i] = null;
            ItemNumber--;
        }
        return itemToRemove;
    }

    public void UpdateSelectedItem(GameTime gameTime)
    {   
        if (SelectedItemIndex >= 0 && SelectedItemIndex <= Storage-1)
        {
            if (SelectedItem != null && !SelectedItem.KillMe)
                SelectedItem?.Update(gameTime);
        }
    }

    public void DrawSelectedItem(SpriteBatch _spriteBatch)
    {
        if (SelectedItemIndex >= 0 && SelectedItemIndex < ItemNumber)
        {
            Items[SelectedItemIndex]?.Draw(_spriteBatch);
        }
    }

    public Item RemoveSelectedItem()
    {
        Item itemToRemove = null;
        if (Items[SelectedItemIndex] != null)
        {
            itemToRemove = RemoveItemAt(SelectedItemIndex);
        }
        return itemToRemove;
    }

    public void PrintInventory()
    {
        foreach (Item item in Items)
        {
            Console.WriteLine($"{(item == null ? "Empty" : item)} - ");
        }
    }

    public override string ToString()
    {
        return $"[INVENTORY] Max item number: {Storage}, current item number: {ItemNumber}";
    }
}   