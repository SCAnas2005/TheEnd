
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public static class ItemManager
{
    private static List<Item> _items => [.. EntityManager.Entities.OfType<Item>()];
    public static List<Item> Items { get => _items; }
    public static int Count => Items.Count;


    public static T CreateBasic<T>(Vector2 position, Map map) where T : Item, new()
    {
        T item = new();
        item.Position = position;
        item.Map = map;
        item.Load(Globals.Content);
        return item;
    }

    public static Item CreateBasicFromType(Type t, Vector2 position, Map map)
    {
        if (typeof(Item).IsAssignableFrom(t))
        {
            // Crée dynamiquement une instance du type concret choisi
            Item item = (Item)Activator.CreateInstance(t);
            item.Position = position;
            item.Map = map;
            item.debug = true;
            item.Load(Globals.Content);

            return item;
        }

        return null;
    }

    public static void AddItem<T>(T Item) where T : Item
    {
        EntityManager.AddEntity(Item);
    }

    public static void AddAndLoad<T>(T Item) where T : Item
    {
        Item.Load(Globals.Content);
        AddItem(Item);
    }

    public static void RemoveItem<T>(T item) where T : Item
    {
        EntityManager.RemoveEntity(item);
    }

    public static List<T> GetAll<T>()
    {
        return [.. _items.OfType<T>()];
    }

    public static List<Item> GetEntitiesOfType<T>() where T : class
    {
        return [.. _items.Where(e => e is T)];
    }

    public static List<Item> GetEntitiesWhere(Func<Item, bool> predicate)
    {
        return [.. _items.Where(predicate)];
    }

    public static Item GetFirst(Func<Item, bool> predicate)
    {
        return _items.FirstOrDefault(predicate);
    }


}