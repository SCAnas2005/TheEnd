
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
    private Rectangle _InfoRectRight;
    private InventoryWidget _inventoryWidget;
    private UserInfoWidget _userInfo;

    private (int, int) playerPos;

    private Dictionary<MapScene, Map> _maps;
    private MapScene _mapScene;

    private Map CurrentMapScene {get {return _maps[_mapScene];}}

    public Dictionary<MapScene, List<InteractionObject>> InteractionObjects;

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

        _InfoRect = new Rectangle(_rect.X, _rect.Height - 200, _rect.Width, 200);
        _InfoRectRight = new Rectangle(_rect.Width - 400, _rect.Y, 400, _rect.Height);

        InteractionObjects = [];
        CreateAllMaps();
    }

    public void CreateAllMaps()
    {
        MapScene i = MapScene.City1;
        _maps[i] = new Map(rect: Rectangle.Empty, src:"Maps/map", name: "City1", scene: i, debug:false);
        i = MapScene.Home;
        _maps[i] = new Map(rect: Rectangle.Empty, src: "Maps/home_map", name: "Home", scene: i, debug:false);

        _mapScene = MapScene.City1;
    }


    public void ChangeMapScene(MapScene scene, Vector2? newPlayerPos = null)
    {
        _mapScene = scene;
        if (!_maps[_mapScene].Loaded)
        {
            _maps[_mapScene].Load(Globals.Content);
            CreateItemsForScene(_mapScene);
            CreateInteractionsForMapScene(_mapScene);
        }
        if (newPlayerPos != null)
            _player.Position = newPlayerPos.Value;
        _player.SetNewMap(CurrentMapScene);

        Console.WriteLine($"Changing map to {_mapScene}");
    }


    public void CreateInteractionsForMapScene(MapScene scene)
    {
        if (InteractionObjects.ContainsKey(scene)) { return; }
        InteractionObjects[scene] = [];
        foreach (var obj in _maps[scene].TiledMap.GetLayer<TiledMapObjectLayer>("Interaction").Objects)
        {
            InteractionObject i = null;
            if (scene == MapScene.City1)
            {
                if (obj.Name == "home_door" && obj.Properties["type"] == "door")
                {
                    i = new TransitionDoorObject(
                        rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                        type: obj.Properties["type"],
                        mapScene: scene,
                        name: obj.Name,
                        l: int.Parse(obj.Properties["destinationL"]),
                        c: int.Parse(obj.Properties["destinationC"]),
                        destinationMap: Utils.StringMapNameToMapScene(obj.Properties["destinationMap"])
                    );
                }
            }
            else if (scene == MapScene.Home)
            {
                if (obj.Name == "back_home_door" && obj.Properties["type"] == "door")
                {
                    i = new TransitionDoorObject(
                        rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                        type: obj.Properties["type"],
                        mapScene: MapScene.Home,
                        name: obj.Name,
                        l: int.Parse(obj.Properties["destinationL"]),
                        c: int.Parse(obj.Properties["destinationC"]),
                        destinationMap: Utils.StringMapNameToMapScene(obj.Properties["destinationMap"]),
                        actionName: () => "[Sortir]",
                        actionInstructions: () => "Appuyer sur [E] pour [Sortir]"
                    );
                }
                if (obj.Name.Contains("home_door") && obj.Properties["type"] == "normal_door")
                {
                    i = new NormalDoorObject(
                        rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                        type: obj.Properties["type"],
                        mapScene: MapScene.Home,
                        name: obj.Name,
                        l: int.Parse(obj.Properties["posL"]),
                        c: int.Parse(obj.Properties["posC"]),
                        state: bool.Parse(obj.Properties["state"]),
                        locked: bool.Parse(obj.Properties["locked"])
                    );
                }
                if (obj.Name == "table_paper" && obj.Properties["type"] == "readable_paper")
                {
                    i = new ReadablePaperObject(
                        rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                        type: obj.Properties["type"],
                        mapScene: MapScene.Home,
                        name: obj.Name,
                        l: int.Parse(obj.Properties["posL"]),
                        c: int.Parse(obj.Properties["posC"]),
                        content: File.ReadAllText("data/letter.txt"),
                        actionName: () => "[Lire]",
                        actionInstructions: () => "Appuyer sur [E] pour [LIRE]"
                    );
                }
                // if (obj.Name == "home_door_2" && obj.Properties["type"] == "normal_door")
                // {
                //     i = new NormalDoorObject(
                //         rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                //         type: obj.Properties["type"],
                //         mapScene: MapScene.Home,
                //         name: obj.Name,
                //         l: int.Parse(obj.Properties["posL"]),
                //         c: int.Parse(obj.Properties["posC"]),
                //         state: bool.Parse(obj.Properties["state"]),
                //         locked: bool.Parse(obj.Properties["locked"])
                //     );
                // }
                // if (obj.Name == "home_door_3" && obj.Properties["type"] == "normal_door")
                // {
                //     i = new NormalDoorObject(
                //         rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                //         type: obj.Properties["type"],
                //         mapScene: MapScene.Home,
                //         name: obj.Name,
                //         l: int.Parse(obj.Properties["posL"]),
                //         c: int.Parse(obj.Properties["posC"]),
                //         state: bool.Parse(obj.Properties["state"]),
                //         locked: bool.Parse(obj.Properties["locked"])
                //     );
                // }
                if (obj.Name == "home_armoire_cle" && obj.Properties["type"] == "armoire")
                {
                    i = new ArmoireObject(
                        rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                        type: obj.Properties["type"],
                        mapScene: MapScene.Home,
                        name: obj.Name,
                        l: int.Parse(obj.Properties["posL"]),
                        c: int.Parse(obj.Properties["posC"]),
                        item: obj.Properties["content"] == "" ? null : _items.FirstOrDefault(itm => itm.Name == "Secret key")
                    );
                }

                if (obj.Name == "home_armoire_timmy" && obj.Properties["type"] == "armoire")
                {
                    i = new ArmoireObject(
                        rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                        type: obj.Properties["type"],
                        mapScene: MapScene.Home,
                        name: obj.Name,
                        l: int.Parse(obj.Properties["posL"]),
                        c: int.Parse(obj.Properties["posC"]),
                        item: obj.Properties["content"] == "" ? null : _items.FirstOrDefault(itm => itm.Name == "Photo de timmy")
                    );
                }

                if (obj.Name.Contains("vide") && obj.Properties["type"] == "armoire")
                {
                    i = new ArmoireObject(
                        rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                        type: obj.Properties["type"],
                        mapScene: MapScene.Home,
                        name: obj.Name,
                        l: int.Parse(obj.Properties["posL"]),
                        c: int.Parse(obj.Properties["posC"]),
                        item: null
                    );
                }
            }
            if (i != null) InteractionObjects[scene].Add(i);
        }
    }

    public void CreateItemsForScene(MapScene scene)
    {
        Item i;
        if (scene == MapScene.Home)
        {
            i = new Weapon(
                rect: new Rectangle(
                    x: (int)Map.GetPosFromMap((5, 20), CurrentMapScene.TileSize).X,
                    y: (int)Map.GetPosFromMap((5, 20), CurrentMapScene.TileSize).Y,
                    width: CurrentMapScene.TileSize.Width,
                    height: CurrentMapScene.TileSize.Height
                ),
                src: "Weapons/Guns/gun",
                name: "Gun",
                map: _maps[scene],
                mapScene: scene
            );

            i.Load(Globals.Content);
            _items.Add(i);

            i = new KeyItem(new Rectangle(120, 20, 16, 10), "Items/secret_key", name: "Secret key", doorToUnlock: "home_door_2", _maps[_mapScene], _mapScene, dropped: false);
            i.Load(Globals.Content);
            _items.Add(i);
            
            i = new PhotoItem(
                rect: new Rectangle(
                    (int)Map.GetPosFromMap((5, 13), _maps[_mapScene].TileSize).X,
                    (int)Map.GetPosFromMap((5, 13), _maps[_mapScene].TileSize).Y,
                    CurrentMapScene.TileSize.Width,
                    CurrentMapScene.TileSize.Height
                ),
                src: "family/timmy",
                name: "Photo de timmy",
                map: _maps[scene],
                mapScene: scene,
                dropped: false
            );

            i.Load(Globals.Content);
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
            name: "Jason",
            speed: 5,
            health: 100,
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
                health: 50,
                map: CurrentMapScene,
                mapScene: MapScene.City1,
                debug: true
            );
            zombie.player = _player;
            _zombies.Add(zombie);
        }

        CreateItemsForScene(_mapScene);
        CreateInteractionsForMapScene(_mapScene);

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

        _inventoryWidget = new InventoryWidget(
            rect: new Rectangle(_InfoRect.X + 10, _InfoRect.Y + 10, _InfoRect.Width - 10, _InfoRect.Height),
            inventory: _player.Inventory
        );

        _userInfo = new UserInfoWidget(
            rect: _InfoRectRight,
            player: _player,
            debug:true
        );

        _inventoryWidget.Load(Content);
        _userInfo.Load(Content);
    }

    public override void Update(GameTime gameTime)
    {
        CurrentMapScene.Update(gameTime);
        var currentScene = _mapScene;

        if (InteractionObjects.TryGetValue(currentScene, out var objects))
        {
            foreach (var obj in objects.ToList()) // .ToList() évite aussi les erreurs de modification
            {
                obj.Update(gameTime, CurrentMapScene, _player);

                // Si la scène a changé (par ex : par une porte), on arrête ici
                if (_mapScene != currentScene)
                    break;
            }
        }
        _player.Update(gameTime);
        foreach (var itm in _items)
        {
            if (itm.MapScene == _mapScene && itm.IsDropped)
            {
                itm.Update(gameTime, _player, CurrentMapScene);
            }
        }
        foreach (var z in _zombies)
        {
            if (z.MapScene == _mapScene)
            {
                z.Update(gameTime);
            }
        }



        _inventoryWidget.Update(gameTime);
        _userInfo.Update(gameTime); 
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        _spriteBatch.End();
        _spriteBatch.Begin(transformMatrix: CurrentMapScene.ScaleMatrix);
        CurrentMapScene.Draw(_spriteBatch);
        foreach (var itm in _items)
        {
            if (itm.MapScene == _mapScene && itm.IsDropped)
            {
                itm.Draw(_spriteBatch);
            }
        }
        _player.Draw(_spriteBatch);
        foreach (var obj in InteractionObjects[_mapScene])
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
        Shape.DrawRectangle(_spriteBatch, CurrentMapScene.Rect, Color.Red);
        _spriteBatch.End();
        _spriteBatch.Begin();
        _inventoryWidget.Draw(_spriteBatch);
        _userInfo.Draw(_spriteBatch);
        if (_debug)
        {
            DrawDebug(_spriteBatch);
        }
    }
}