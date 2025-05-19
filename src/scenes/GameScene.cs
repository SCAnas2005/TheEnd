
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using TheEnd;

public enum MapScene {
    City1,
    Home,
}

public class GameScene : Scene {

    private Rectangle _InfoRect;

    private (int, int) playerPos;

    private Dictionary<MapScene, Map> _maps;
    private MapScene _mapScene;

    private Map CurrentMapScene {get {return _maps[_mapScene];}}

    public List<InteractionObject> InteractionObjects;

    private Chrono _chrono;

    private Player _player;
    public List<Zombies> _zombies;


    public List<Item> _items;


    public GameScene(SceneState screenState, Rectangle rect, bool debug = false, Action OnClose = null) : base(screenState: screenState, rect: rect, debug: debug, OnClose: OnClose)
    {

        _maps = [];
        playerPos = (10, 10);
        _zombies = [];
        _items = [];

        _InfoRect = new Rectangle(_rect.X + _rect.Width - 200, _rect.Y, 200, _rect.Height);
        InteractionObjects = [];
        CreateAllMaps();
    }

    public void CreateAllMaps()
    {
        MapScene i = MapScene.City1;
        _maps[i] = new Map(rect: _rect, src:"Maps/map", name: "City1", scene: i);
        i = MapScene.Home;
        _maps[i] = new Map(rect: _rect, src: "Maps/home_map", name: "Home", scene: i);

        _mapScene = MapScene.City1;
    }


    public void ChangeMapScene(MapScene scene, Vector2? newPlayerPos = null)
    {
        _mapScene = scene;
        if (!_maps[_mapScene].Loaded)
        {
            _maps[_mapScene].Load(Globals.Content);
            CreateItems();
        }
        if (newPlayerPos != null)
            _player.Position = newPlayerPos.Value;
        _player.SetNewMap(CurrentMapScene);
        GetInteractions();
    }


    public void GetInteractions()
    {
        InteractionObjects = [];
        foreach (var obj in CurrentMapScene.TiledMap.GetLayer<TiledMapObjectLayer>("Interaction").Objects)
        {
            InteractionObject i = null;
            if (obj.Name == "home_door" && obj.Properties["type"] == "door")
            {
                i = new TransitionDoorObject(
                    rect: new Rectangle(_rect.X+(int)obj.Position.X, _rect.Y+(int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    l: int.Parse(obj.Properties["destinationL"]),
                    c: int.Parse(obj.Properties["destinationC"]),
                    destinationMap: Utils.StringMapNameToMapScene(obj.Properties["destinationMap"])
                );
            } 
            if (obj.Name == "back_home_door" && obj.Properties["type"] == "door")
            {
                i = new TransitionDoorObject(
                    rect: new Rectangle(_rect.X+(int)obj.Position.X, _rect.Y+(int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    l: int.Parse(obj.Properties["destinationL"]),
                    c: int.Parse(obj.Properties["destinationC"]),
                    destinationMap: Utils.StringMapNameToMapScene(obj.Properties["destinationMap"]),
                    actionName: () => "[Sortir]",
                    actionInstructions: () => "Appuyer sur [E] pour [Sortir]"
                );
            } 
            if (obj.Name == "home_door_1" && obj.Properties["type"] == "normal_door")
            {
                i = new NormalDoorObject(
                    rect: new Rectangle(_rect.X+(int)obj.Position.X, _rect.Y+(int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    l: int.Parse(obj.Properties["posL"]),
                    c: int.Parse(obj.Properties["posC"]),
                    state: bool.Parse(obj.Properties["state"])
                );
            }
            if (obj.Name == "table_paper" && obj.Properties["type"] == "readable_paper")
            {
                i = new ReadablePaperObject(
                    rect: new Rectangle(_rect.X+(int)obj.Position.X, _rect.Y+(int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    l: int.Parse(obj.Properties["posL"]),
                    c: int.Parse(obj.Properties["posC"]),
                    content: File.ReadAllText("data/letter.txt"),
                    actionName: () => "[Lire]",
                    actionInstructions: () => "Appuyer sur [E] pour [LIRE]"
                );
            }
            // if (obj.Name == "gun_1" && obj.Properties["type"] == "gun")
            // {
            //     i = new GunObject(
            //         rect: new Rectangle(_rect.X+(int)obj.Position.X, _rect.Y+(int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
            //         type: obj.Properties["type"],
            //         l: int.Parse(obj.Properties["posL"]),
            //         c: int.Parse(obj.Properties["posC"]),
            //         name: obj.Name,
            //         actionName: () => "[Prendre]",
            //         actionInstructions: () => "Appuyer sur [E] pour [Prendre]"
            //     );
            // }
            if (i != null) InteractionObjects.Add(i);
        }
    }

    public void CreateItems()
    {
        Item i;
        if (_mapScene == MapScene.City1)
        {
            i = new Weapon(
                rect: new Rectangle(
                    x: (int)Map.GetPosFromMap((6, 13), CurrentMapScene.TileSize).X,
                    y: (int)Map.GetPosFromMap((6, 13), CurrentMapScene.TileSize).Y,
                    width: CurrentMapScene.TileSize.Width,
                    height: CurrentMapScene.TileSize.Height
                ),
                src: "Weapons/Guns/gun",
                name: "Gun",
                map: CurrentMapScene,
                mapScene: _mapScene
            );

            _items.Add(i);
        }
    }

    public override void Load(ContentManager Content)
    {
        CurrentMapScene.Load(Content);
        Console.WriteLine($"Map is loaded: {CurrentMapScene.Loaded}");
        _player = new Player(
            rect: new Rectangle(
                playerPos.Item2 * CurrentMapScene.TileSize.Width,
                playerPos.Item1 * CurrentMapScene.TileSize.Height,
                CurrentMapScene.TileSize.Width - 3,
                CurrentMapScene.TileSize.Height - 3
            ),
            src: "Player/idle",
            speed: 5,
            map: CurrentMapScene,
            debug: true
        );

        var pos = CurrentMapScene.GetAllWalkablesPosition();
        for (int i = 0; i < 1; i++)
        {
            var randomPos = (17, 15);//pos[Utils.Random.Next(0, pos.Count)];
            var zombie = new Zombies(
                rect: new Rectangle(
                    randomPos.Item2 * CurrentMapScene.TileSize.Width,
                    randomPos.Item1 * CurrentMapScene.TileSize.Height,
                    CurrentMapScene.TileSize.Width - 3,
                    CurrentMapScene.TileSize.Height - 3
                ),
                src: "",
                speed: 3,
                map: CurrentMapScene,
                mapScene: MapScene.City1,
                debug: true
            );
            zombie.player = _player;
            _zombies.Add(zombie);
        }

        GetInteractions();
        CreateItems();

        _player.Map = CurrentMapScene;
        _player.Load(Content);
        foreach (var z in _zombies)
        {
            z.Load(Content);
        }
        foreach (var itm in _items)
        {
            itm.Load(Content);
        }
    }

    public override void Update(GameTime gameTime)
    {
        CurrentMapScene.Update(gameTime);
        foreach (var obj in InteractionObjects)
        {
            obj.Update(gameTime, CurrentMapScene, _player);

        }
        foreach (var itm in _items)
        {
            if (itm.MapScene == _mapScene && itm.IsDropped)
            {
                itm.Update(gameTime, _player, CurrentMapScene);
            }
        }
        _player.Update(gameTime);
        foreach (var z in _zombies)
        {
            if (z.MapScene == _mapScene)
            {
                z.Update(gameTime);
            }
        }
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.End();
        _spriteBatch.Begin(transformMatrix: CurrentMapScene.ScaleMatrix);
        CurrentMapScene.Draw(_spriteBatch);
        foreach (var itm in _items)
        {
            Console.WriteLine($"Item: {itm.IsDropped}, {itm.MapScene}, currentMapScene: {_mapScene}");
            if (itm.MapScene == _mapScene && itm.IsDropped)
            {
                itm.Draw(_spriteBatch);
            }
        }
        _player.Draw(_spriteBatch);
        foreach (var obj in InteractionObjects)
        {
            obj.Draw(_spriteBatch);
            
        }
        foreach (var z in _zombies)
        {
            if (z.MapScene == _mapScene)
            {
                z.Draw(_spriteBatch);
            }
        }
        _spriteBatch.End();
        _spriteBatch.Begin();
        if (_debug)
        {
            DrawDebug(_spriteBatch);
        }

    }
}