
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;

public static class NpcManager
{
       public static List<Npc> Npcs 
        => [.. EntityManager.Entities.OfType<Npc>()];
    public static List<NpcConfig> NpcConfigs = [];
    public static int Count => Npcs.Count;

    public static void Init()
    {
        var jsonFile = "config/npcs/npcs.json";
        string json = File.ReadAllText(jsonFile);
        NpcConfigs = JsonSerializer.Deserialize<List<NpcConfig>>(json);
    }

    public static void AddNpc<T>(T npc) where T : Npc
    {
        EntityManager.AddEntity(npc);
    }

    public static void RemoveNpc<T>(T npc) where T : Npc
    {
        EntityManager.RemoveEntity(npc);
    }

    public static T CreateBasic<T>(Vector2 position, Map map) where T : Npc, new()
    {
        T npc = new();

        return npc;
    }

    public static Npc CreateBasicFromType(Type t, Vector2 position, Map map)
    {
        if (typeof(Npc).IsAssignableFrom(t))
        {
            // Crée dynamiquement une instance du type concret choisi
            Npc npc = (Npc)Activator.CreateInstance(t);
            npc.Position = position;
            npc.SpawnPoint = position;
            npc.Map = map;
            npc.Config = GetRandomConfig();
            npc.Load(Globals.Content);
            return npc;
        }

        return CreateBasicNpcFromType(t, position, map, GetRandomConfig());
    }

    public static Npc CreateBasicNpcFromType(Type t, Vector2 position, Map map, NpcConfig config)
    {
        if (typeof(Npc).IsAssignableFrom(t))
        {
            // Crée dynamiquement une instance du type concret choisi
            Npc npc = (Npc)Activator.CreateInstance(t);
            npc.Position = position;
            npc.SpawnPoint = position;
            npc.Map = map;
            npc.Config = config;
            npc.Load(Globals.Content);
            return npc;
        }
        
        return null;
    }

    public static List<Npc> CreateUselessNpcs(int number = 1)
    {
        if (number <= 0) return [];
        List<Npc> npcs = new();

        string[] randomNames = ["James", "Nathan", "Claire", "Lucie", "Thomas", "Jean"];

        for (int i = 0; i < number; i++)
        {
            var config = GetRandomConfig();
            config.Name = randomNames[Utils.Random.Next(0, randomNames.Length)];
            var npc = new Npc(
                rect: Rectangle.Empty,
                config: config,
                src: "", speed: 2f, health: 100, map: null, debug: true
            );
            npcs.Add(npc);
        }

        return npcs;
    }

    public static NpcConfig GetConfigByName(string name)
    {
        NpcConfig config = null;
        foreach (var c in NpcConfigs)
        {
            if (c.Name == name) return c;
        }
        return config;
    }

    public static NpcConfig GetRandomConfig()
    {
        int r = Utils.Random.Next(0, NpcConfigs.Count);
        return NpcConfigs[r];
    }
    

    public static List<T> GetAll<T>()
    {
        return [.. Npcs.OfType<T>()];
    }

    public static List<Npc> GetEntitiesOfType<T>() where T : class
    {
        return [.. Npcs.Where(e => e is T)];
    }

    public static List<Npc> GetEntitiesWhere(Func<Npc, bool> predicate)
    {
        return [.. Npcs.Where(predicate)];
    }

    public static Npc GetFirst(Func<Npc, bool> predicate)
    {
        return Npcs.FirstOrDefault(predicate);
    }
}