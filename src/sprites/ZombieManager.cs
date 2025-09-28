
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Xna.Framework;

public static class ZombieManager
{
    private static List<Zombies> _zombies => [.. EntityManager.Entities.OfType<Zombies>()];
    public static List<Zombies> Zombies => _zombies;
    public static int Count => Zombies.Count;

    private static double _lastSpawnTime = 0;
    private static double MAX_SPAWN_COOLDOWN = 20000;
    private static double MIN_SPAWN_COOLDOWN = 10000;
    private static double _cooldown = MIN_SPAWN_COOLDOWN;


    public static void AddZombie<T>(T zombie) where T : Zombies
    {
        EntityManager.AddEntity(zombie);
    }

    public static void RemoveNpc<T>(T zombie) where T : Zombies
    {
        EntityManager.RemoveEntity(zombie);
    }

    public static Zombies CreateBasicZombie(Vector2 position, Map map)
    {
        var zombieRect = new Rectangle(
            (int)position.X,
            (int)position.Y,
            Sprite.GetSpriteSize(map).Width,
            Sprite.GetSpriteSize(map).Height
        );
        var zombie = new Zombies(
            rect: zombieRect,
            src: "",
            speed: 2,
            health: 50,
            map: map,
            debug: true
        );

        zombie.Load(Globals.Content);

        return zombie;
    }

    public static T CreateBasic<T>(Vector2 position, Map map) where T : Zombies, new()
    {
        T z = new()
        {
            Position = position,
            Map = map
        };
        z.Load(Globals.Content);
        return z;
    }

    public static Zombies CreateBasicFromType(Type t, Vector2 position, Map map)
    {
        if (typeof(Zombies).IsAssignableFrom(t))
        {
            // Crée dynamiquement une instance du type concret choisi
            var z = (Zombies)Activator.CreateInstance(t);
            z.Position = position;
            z.Map = map;
            z.Load(Globals.Content);

            return z;
        }

        return null;
    }

    public static Zombies SpawnBasicZombieNearPlayer()
    {
        var map = EntityManager.Player.Map;

        // Récupère les tiles marchables autour de la position
        var walkableTiles = map.GetWalkablesPositionFromPosition(EntityManager.Player.Position, new Size(5, 5));

        if (walkableTiles.Count == 0)
            return null; // Aucun endroit pour spawn

        // Choisis une tile aléatoire
        var random = new Random();
        var randomPos = walkableTiles[random.Next(walkableTiles.Count)];

        Vector2 pos = new Vector2(randomPos.col * map.TileSize.Width, randomPos.row * map.TileSize.Height);
        // Convertit la position en pixels
        var zombie = CreateBasicZombie(pos, map);
        // Ajoute-le à la scène ou à ton EntityManager
        AddZombie(zombie);

        return zombie;
    }


    public static List<T> GetAll<T>()
    {
        return [.. _zombies.OfType<T>()];
    }

    public static List<Zombies> GetEntitiesOfType<T>() where T : class
    {
        return [.. _zombies.Where(e => e is T)];
    }

    public static List<Zombies> GetEntitiesWhere(Func<Zombies, bool> predicate)
    {
        return [.. _zombies.Where(predicate)];
    }

    public static Zombies GetFirst(Func<Zombies, bool> predicate)
    {
        return _zombies.FirstOrDefault(predicate);
    }



    public static void SpawnZombiesGroup(int n, Vector2 position)
    {
        var map = EntityManager.Player.Map;

        // Récupère les tiles marchables autour de la position
        var walkableTiles = map.GetWalkablesPositionFromPosition(position, new Size(20, 20));

        if (walkableTiles.Count == 0)
            return; // Aucun endroit pour spawn

        // Choisis une tile aléatoire
        var random = new Random();
        for (int i = 0; i < n; i++)
        {
            var randomPos = walkableTiles[random.Next(walkableTiles.Count)];
            Vector2 pos = new Vector2(randomPos.col * map.TileSize.Width, randomPos.row * map.TileSize.Height);

            var z = CreateBasicZombie(pos, map);
            AddZombie(z);
        }
    }

    public static void Update(GameTime gameTime)
    {
        var player = EntityManager.Player;
        double totalMs = gameTime.TotalGameTime.TotalMilliseconds;
        if (player.Map.InZombieSpawningZone(player.Rect))
        {
            if (totalMs - _lastSpawnTime >= _cooldown)
            {
                SpawnZombiesGroup(n: Utils.Random.Next(3, 6), position: player.Position);
                _lastSpawnTime = totalMs;

                _cooldown = Utils.NextDouble(MIN_SPAWN_COOLDOWN, MAX_SPAWN_COOLDOWN);
                NotificationManager.Add(new Notification(
                    text: "A group of zombie has spawned near to you",
                    font: CFonts.Minecraft_24
                ));
            }
        }

    }
}