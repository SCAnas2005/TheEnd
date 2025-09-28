
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public static class EntityManager
{
    private static List<Entity> _entities = new();
    public static List<Entity> Entities { get => _entities; }

    public static int Count => Entities.Count;

    public static Player Player;
    

    public static Dictionary<string, Type> GetPublicPropertiesOfType(Type t)
    {
        Dictionary<string, Type> properties = [];
        var entityProperties = typeof(Entity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        Console.WriteLine("==== Entity =====================");
        foreach (var p in entityProperties)
        {
            properties[p.Name] = p.PropertyType;
            Console.WriteLine($"{p.Name} : {p.PropertyType}");
        }

        Console.WriteLine($"\n==== {t.Name} =====================");

        var targetProperties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var p in targetProperties)
        {
            properties[p.Name] = p.PropertyType;
            Console.WriteLine($"{p.Name} : {p.PropertyType}");
        }

        return properties;
    }

    public static void AddEntity<T>(T entity) where T : Entity
    {
        _entities.Add(entity);
    }

    public static Entity RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
        return entity;
    }

    public static void RemoveAll()
    {
        _entities.Clear();
    }

    public static void RemoveAllExcept(List<Type> t=null)
    {
        if (t == null || t.Count == 0)
        {
            RemoveAll(); return;
        }
        Entities.RemoveAll(e => !t.Contains(e.GetType()));
    }


    public static List<T> GetAll<T>()
    {
        return [.. _entities.OfType<T>()];
    }

    public static List<Entity> GetEntitiesOfType<T>() where T : class
    {
        return [.. _entities.Where(e => e is T)];
    }

    public static List<Entity> GetEntitiesWhere(Func<Entity, bool> predicate)
    {
        return [.. _entities.Where(predicate)];
    }

    public static List<T> GetEntitiesWhere<T>(Func<Entity, bool> predicate) where T : Entity
    {
        return [.. _entities
            .Where(predicate)
            .OfType<T>()];
    }


    public static Entity GetFirst(Func<Entity, bool> predicate)
    {
        return _entities.FirstOrDefault(predicate);
    }

    public static List<Type> GetAllEntityTypes(Type[] excludes = null)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return [.. assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.IsSubclassOf(typeof(Entity)) &&
                (excludes == null || !excludes.Contains(t))
            )];
    }


    public static T CreateBasic<T>(Vector2 position, Map map) where T : Entity, new()
    {
        T z = new()
        {
            Position = position,
            Map = map
        };
        z.Load(Globals.Content);
        return z;
    }

    public static Entity CreateBasicFromType(Type t, Vector2 position, Map map)
    {
        if (typeof(Entity).IsAssignableFrom(t))
        {
            Entity e = (Entity)Activator.CreateInstance(t);
            e.Position = position;
            e.Map = map;
            e.Load(Globals.Content);
            return e;
        }
        return null;
    }

    public static void LoadEntities(ContentManager Content)
    {
        foreach (var e in _entities)
        {
            e.Load(Content);
        }
    }


    public static void Update(GameTime gameTime, Map currentMap)
    {
        List<Entity> entityToRemove = new List<Entity>();
        bool itemInInteraction = false;

        for (int i = 0; i < _entities.Count; i++)
        {
            Entity ent = _entities[i];

            // Items
            if (ent is Item item)
            {
                if (itemInInteraction) continue;
                if (item.Map.Scene == currentMap.Scene && item.IsDropped)
                    item.Update(gameTime);
                else
                    item.UpdateOffscreen(gameTime);

                if (item.KillMe)
                    entityToRemove.Add(item);

                if (item.IsInteracting() && item.IsDropped)
                    itemInInteraction = true; // stop l'update des entités si un item est en interaction
            }
            // Zombies
            else if (ent is Zombies zombie)
            {
                if (zombie.Map.Scene == currentMap.Scene)
                    zombie.Update(gameTime);
                else
                    zombie.UpdateOffscreen(gameTime);

                if (zombie.IsDead || zombie.KillMe)
                    entityToRemove.Add(zombie);
            }
            // Autres entités
            else
            {
                if (ent.Map.Scene == currentMap.Scene)
                    ent.Update(gameTime);
                else
                    ent.UpdateOffscreen(gameTime);

                if (ent.KillMe)
                    entityToRemove.Add(ent);
            }
        }

        // Suppression en dehors de la boucle
        foreach (var e in entityToRemove)
            _entities.Remove(e);

        ZombieManager.Update(gameTime);
    }

    public static void Draw(SpriteBatch _spriteBatch, Map currentMap)
    {
        var entityToDraw = _entities
        .Where(e => e.Map.Scene == currentMap.Scene)
        .OrderBy(e => e.zIndex)
        .ToList();

        foreach (var entity in entityToDraw)
        {
            if (entity.Map.Scene != currentMap.Scene) continue;
            if (entity is Item item)
            {
                if (item.Map.Scene == currentMap.Scene && item.IsDropped)
                {
                    item.Draw(_spriteBatch);
                }
            }
            else if (entity.Map.Scene == currentMap.Scene)
            {
                entity.Draw(_spriteBatch);
            }
        }
    }
}