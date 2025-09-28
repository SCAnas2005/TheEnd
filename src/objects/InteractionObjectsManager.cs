
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class InteractionObjectsManager
{
    public static Dictionary<MapScene, List<InteractionObject>> InteractionObjects;

    public static void Init()
    {
        InteractionObjects = [];
    }

    public static void Add(MapScene scene, InteractionObject i)
    {
        if (!InteractionObjects.TryGetValue(scene, out List<InteractionObject> value))
        {
            value = [];
            InteractionObjects[scene] = value;
        }

        value.Add(i);
    }

    public static void Remove(MapScene scene, InteractionObject i)
    {
        if (!InteractionObjects.TryGetValue(scene, out List<InteractionObject> value)) return;
        value.Remove(i);
    }

    public static bool HasScene(MapScene scene)
    {
        return InteractionObjects.ContainsKey(scene);
    }

    public static T GetFirst<T>(MapScene scene) where T : InteractionObject
    {
        if (!HasScene(scene)) return null;
        return InteractionObjects[scene].OfType<T>().FirstOrDefault();
    }

    public static List<T> GetAll<T>(MapScene scene) where T : InteractionObject
    {
        if (!HasScene(scene)) return null;
        return [.. InteractionObjects[scene].OfType<T>()];
    }


    public static void Update(GameTime gameTime, Map currentMap)
    {
        var player = EntityManager.Player;
        List<InteractionObject> objectsToRemove = [];

        var sceneAtStart = currentMap.Scene;
        if (InteractionObjects.TryGetValue(currentMap.Scene, out var objects))
        {
            sceneAtStart = currentMap.Scene; // snapshot de la scène au début

            foreach (var obj in objects.ToList())
            {
                obj.Update(gameTime, currentMap, player);
                if (obj.IsDestroyed) objectsToRemove.Add(obj);

                // si la scène a changé en cours de route, on stoppe la boucle
                if (currentMap.Scene != sceneAtStart)
                    break;
            }
        }


        foreach (var o in objectsToRemove)
        {
            Remove(sceneAtStart, o);
        }
    }

    public static void Draw(SpriteBatch _spriteBatch, Map CurrentMap)
    {
        if (!HasScene(CurrentMap.Scene)) return;
        foreach (var io in InteractionObjects[CurrentMap.Scene])
        {
            io.Draw(_spriteBatch);
        }   
    }
}