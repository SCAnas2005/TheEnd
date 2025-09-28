

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

public static class SceneManager
{
    public static SceneState SceneState;
    public static Dictionary<SceneState, Scene> Scenes;
    public static Scene CurrentScene;

    public static void PrintInfo()
    {
        Console.WriteLine($"[SCENE] SceneState: {SceneState}");
    }

    public static void Init(SceneState sceneState = SceneState.Menu)
    {
        Scenes = new Dictionary<SceneState, Scene>{};
        SceneState = sceneState;
        CreateAllScenes();
        CurrentScene = Scenes[SceneState];
    }

    public static Scene GetScene(SceneState sceneStateToGet)
    {
        if (!Scenes.ContainsKey(sceneStateToGet)) return null;
        return Scenes[sceneStateToGet];
    }

    public static void AddScene(SceneState newSceneState, Scene newScene)
    {
        Scenes[newSceneState] = newScene;
    }

    public static void RemoveScene(SceneState sceneStateToRemove)
    {
        Scenes.Remove(sceneStateToRemove);
    }

    public static void ChangeScreen(SceneState newScene)
    {
        Console.WriteLine($"[{SceneState}] => [{newScene}]");
        SceneState = newScene;
        CurrentScene = Scenes[SceneState];
    }

    public static void CreateAllScenes()
    {
        int screenWidth = Globals.ScreenSize.Width;
        int screenHeight = Globals.ScreenSize.Height;
        Scenes[SceneState.Game] = new GameScene(SceneState.Game, new Rectangle(0,0,screenWidth, screenHeight), true);
    }

    public static void LoadAllScreens(ContentManager Content)
    {
        foreach (var scene in Scenes)
        {
            scene.Value.Load(Content);
        }
    }
}