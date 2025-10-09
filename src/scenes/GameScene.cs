
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using TheEnd;

public enum MapScene
{
    Labo,
    City1,
    Home,
    Grange1,
    GasStation1,
}

public class GameScene : Scene {

    private Rectangle _InfoRect;
    private Rectangle _InfoRectRight;
    private InventoryWidget _inventoryWidget;
    private UserInfoWidget _userInfo;

    private Dictionary<MapScene, Map> _maps;
    public Dictionary<MapScene, Map> Maps { get => _maps; }
    private Dictionary<MapScene, bool> _itemsCreated;
    private Dictionary<MapScene, bool> _entitiesCreated;
    private MapScene _mapScene;

    public Map CurrentMap {get {return _maps[_mapScene];}}

    // public Dictionary<MapScene, List<InteractionObject>> InteractionObjects;

    public Player _player;


    public List<Vector2> path = null;


    public GameScene(SceneState screenState, Rectangle rect, bool debug = false, Action OnClose = null) : base(screenState: screenState, rect: rect, debug: debug, OnClose: OnClose)
    {
        _maps = [];
        _itemsCreated = [];
        _entitiesCreated = [];


        _InfoRect = new Rectangle(_rect.X, _rect.Height - 200, _rect.Width, 200);
        _InfoRectRight = new Rectangle(_rect.Width - 400, _rect.Y, 400, _rect.Height);

        // InteractionObjects = [];
        InteractionObjectsManager.Init();
        CreateAllMaps();
    }

    public void ReInit()
    {
        _maps = [];
        _itemsCreated = [];
        _entitiesCreated = [];
        _InfoRect = new Rectangle(_rect.X, _rect.Height - 200, _rect.Width, 200);
        _InfoRectRight = new Rectangle(_rect.Width - 400, _rect.Y, 400, _rect.Height);
        InteractionObjectsManager.Init();

        EntityManager.RemoveAllExcept([typeof(Player)]);
        CreateAllMaps();
        Load(Globals.Content);
    }

    public void ReinitMapScene(MapScene scene)
    {
        EntityManager.RemoveWhere((e) => e.Map.Scene == scene && !(e is Player));
        CreateMapScene(scene);
        LoadMap(scene);
    }


    public void CreateAllMaps()
    {
        foreach (var scene in Enum.GetValues<MapScene>())
        {
            CreateMapScene(scene);
        }
        _mapScene = MapScene.City1;
    }

    public void CreateMapScene(MapScene scene)
    {
        switch (scene)
        {
            case MapScene.City1:
                _maps[scene] = new Map(rect: Rectangle.Empty, src: "Maps/map", name: "City1", scene: scene, zoom: 4.5f, debug: false);
                break;

            case MapScene.Home:
                _maps[scene] = new Map(rect: Rectangle.Empty, src: "Maps/home_map", name: "Home", scene: scene, zoom: 1.3f, debug: false);
                break;

            case MapScene.Grange1:
                _maps[scene] = new Map(rect: Rectangle.Empty, src: "Maps/grange_inside", name: "Grange1", scene: scene, zoom: 1.3f, debug: false);
                break;

            case MapScene.GasStation1:
                _maps[scene] = new Map(rect: Rectangle.Empty, src: "Maps/gas_station_map", name: "GasStation1", scene: scene, zoom: 1.3f, debug: false);
                break;
            case MapScene.Labo:
                _maps[scene] = new Map(rect: Rectangle.Empty, src: "Maps/lab", name: "Labo", scene: scene, zoom: 4.5f, debug: false);
                break;
            default:
                break;
        }
        
        _itemsCreated[scene] = false;
        _entitiesCreated[scene] = false;
    }

    public void LoadMap(MapScene scene)
    {
        _maps[scene].Load(Globals.Content);
        if (!_itemsCreated[scene])
            CreateItemsForScene(scene);
        if (!_entitiesCreated[scene])
            CreateEntitiesForScene(scene);
        CreateInteractionsForMapScene(scene);
    }

    public void ChangeMapScene(MapScene scene, Vector2? newPlayerPos = null)
    {
        _mapScene = scene;
        if (!_maps[_mapScene].Loaded)
        {
            LoadMap(_mapScene);
        }
        if (newPlayerPos != null)
            _player.Position = newPlayerPos.Value;
        _player.SetNewMap(CurrentMap);
        Camera2D.Init(Globals.Graphics.GraphicsDevice.Viewport, CurrentMap);
    }


    public void CreateEntitiesForScene(MapScene scene)
    {
        TiledMapObjectLayer entitiesLayer;
        if ((entitiesLayer = _maps[scene].GetAllEntities()) != null)
        {
            Entity i = null;
            foreach (var entity in entitiesLayer.Objects)
            {
                if (entity.Properties["type"] == "barrel")
                {
                    i = new BarrelEntity(
                        rect: new Rectangle(
                            x: (int)Map.GetPosFromMap((int.Parse(entity.Properties["posL"]), int.Parse(entity.Properties["posC"])), CurrentMap.TileSize).X,
                            y: (int)Map.GetPosFromMap((int.Parse(entity.Properties["posL"]), int.Parse(entity.Properties["posC"])), CurrentMap.TileSize).Y,
                            width: CurrentMap.TileSize.Width,
                            height: CurrentMap.TileSize.Height
                        ),
                        map: _maps[scene]
                    );
                }

                if (i != null) { i?.Load(Globals.Content); EntityManager.AddEntity(i); }
            }

            var posS = Map.GetPosFromMap((50,-10), _maps[scene].TileSize);
            var posE = Map.GetPosFromMap((80 ,150), _maps[scene].TileSize);

            Entity fog = new FogEntity(
                rect: new Rectangle(
                    x: (int)posS.X, y: (int)posS.Y,
                    width: (int)(posE-posS).X, height: (int)(posE-posS).Y
                ),
                map: _maps[scene]
            );

            fog.Load(Globals.Content);
            EntityManager.AddEntity(fog);

            _entitiesCreated[scene] = true;
            Console.WriteLine("Entities imported");

        }


        if (scene == MapScene.City1 || scene == MapScene.Labo)
        {
            var npcs = NpcManager.CreateUselessNpcs(100);
            var pos = _maps[scene].GetAllWalkablesPosition();

            // foreach (var npc in npcs)
            // {
            //     var randomPos = pos[Utils.Random.Next(0, pos.Count)];
            //     npc.Rect = new Rectangle(
            //         randomPos.col * CurrentMap.TileSize.Width,
            //         randomPos.row * CurrentMap.TileSize.Height,
            //         Sprite.GetSpriteSize(CurrentMap).Width,
            //         Sprite.GetSpriteSize(CurrentMap).Height
            //     );
            //     npc.Map = _maps[scene];

            //     npc.Load(Globals.Content);
            //     NpcManager.AddNpc(npc);
            // }
        }
    }

    public void CreateInteractionsForMapScene(MapScene scene)
    {
        if (InteractionObjectsManager.HasScene(scene)) return;
        // if (InteractionObjects.ContainsKey(scene)) { return; }
        // InteractionObjects[scene] = [];
        Console.WriteLine("Creating intereactions object for scene : " + scene + ", current map : " + CurrentMap.Scene);
        var interactions = _maps[scene].GetAllInteractions();
        if (interactions == null) return;
        foreach (var obj in interactions.Objects)
        {
            InteractionObject i = null;
            if (obj.Properties["type"] == "door")
            {
                i = new TransitionDoorObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    map: _maps[scene],
                    name: obj.Name,
                    l: obj.Properties.TryGetValue("destinationL", out var lStr) && int.TryParse(lStr, out var lVal) ? lVal : null,
                    c: obj.Properties.TryGetValue("destinationC", out var cStr) && int.TryParse(cStr, out var cVal) ? cVal : null,
                    destinationMap: obj.Properties.TryGetValue("destinationMap", out var dmStr) ? Utils.StringMapNameToMapScene(obj.Properties["destinationMap"]) : null,
                    state: bool.Parse(obj.Properties["state"])
                );
            }
            if (obj.Name == "back_home_door" && obj.Properties["type"] == "door")
            {
                i = new TransitionDoorObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    map: _maps[MapScene.Home],
                    name: obj.Name,
                    l: obj.Properties.TryGetValue("destinationL", out var lStr) && int.TryParse(lStr, out var lVal) ? lVal : null,
                    c: obj.Properties.TryGetValue("destinationC", out var cStr) && int.TryParse(cStr, out var cVal) ? cVal : null,
                    destinationMap: obj.Properties.TryGetValue("destinationMap", out var dmStr) ? Utils.StringMapNameToMapScene(obj.Properties["destinationMap"]) : null,
                    actionName: () => "[Sortir]",
                    actionInstructions: () => "Appuyer sur [E] pour [Sortir]",
                    state: bool.Parse(obj.Properties["state"])
                );
            }
            if (obj.Properties["type"] == "normal_door")
            {
                i = new NormalDoorObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    map: _maps[MapScene.Home],
                    name: obj.Name,
                    l: int.Parse(obj.Properties["posL"]),
                    c: int.Parse(obj.Properties["posC"]),
                    state: bool.Parse(obj.Properties["state"]),
                    key_name: obj.Properties.TryGetValue("key", out var keyname) ? keyname : null,
                    locked: bool.Parse(obj.Properties["locked"])
                );
            }
            if (obj.Name == "table_paper" && obj.Properties["type"] == "readable_paper")
            {
                i = new ReadablePaperObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    map: _maps[MapScene.Home],
                    name: obj.Name,
                    l: int.Parse(obj.Properties["posL"]),
                    c: int.Parse(obj.Properties["posC"]),
                    content: File.ReadAllText("data/letter.txt"),
                    actionName: () => "[Lire]",
                    actionInstructions: () => "Appuyer sur [E] pour [LIRE]"
                );
            }
            if (obj.Name == "home_armoire_cle" && obj.Properties["type"] == "armoire")
            {
                i = new ArmoireObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    map: _maps[MapScene.Home],
                    name: obj.Name,
                    l: int.Parse(obj.Properties["posL"]),
                    c: int.Parse(obj.Properties["posC"]),
                    item: obj.Properties["content"] == "" ? null : EntityManager.GetAll<Item>().FirstOrDefault(itm => itm.Name == "Secret key")
                );
            }

            if (obj.Name == "home_armoire_timmy" && obj.Properties["type"] == "armoire")
            {
                i = new ArmoireObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    map: _maps[MapScene.Home],
                    name: obj.Name,
                    l: int.Parse(obj.Properties["posL"]),
                    c: int.Parse(obj.Properties["posC"]),
                    item: obj.Properties["content"] == "" ? null : EntityManager.GetAll<Item>().FirstOrDefault(itm => itm.Name == "Photo de timmy")
                );
            }

            if (obj.Name.Contains("vide") && obj.Properties["type"] == "armoire")
            {
                i = new ArmoireObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    type: obj.Properties["type"],
                    map: _maps[MapScene.Home],
                    name: obj.Name,
                    l: int.Parse(obj.Properties["posL"]),
                    c: int.Parse(obj.Properties["posC"]),
                    item: null
                );
            }

            if (obj.Properties["type"] == "searchbox")
            {
                i = new SearchObject(
                    rect: new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height),
                    map: _maps[scene],
                    itemToGive: null
                );
            } 
            if (obj.Properties["type"] == "searchbox" && obj.Name == "deadman_grange")
            {
                Rectangle rect = new Rectangle(_rect.X + (int)obj.Position.X, _rect.Y + (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height);
                var item = new BookItem(
                    rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(8).ToPoint()),
                    dropped: false,
                    src: "items/logbook",
                    name: "Logbook",
                    map: CurrentMap
                )
                {
                    IsDropped = false,
                    Content = BookStruct.LoadFromJson("data/logbook1.json")
                };
                ItemManager.AddAndLoad(item);
                i = new SearchObject(
                    rect: rect,
                    map: _maps[scene],
                    itemToGive: item
                );
            } 

            if (i != null)
            {
                i.Load(Globals.Content);
                InteractionObjectsManager.Add(scene, i);
            }
        }
    }

    public void CreateItemsForScene(MapScene scene)
    {
        Item i;
        if (scene == MapScene.City1)
        {
            for (int l = 0; l < 1; l++)
            {
                i = new AmmoItem(
                    rect: new Rectangle(
                        x: (int)Map.GetPosFromMap((10, 5), CurrentMap.TileSize).X,
                        y: (int)Map.GetPosFromMap((10, 5), CurrentMap.TileSize).Y,
                        width: CurrentMap.TileSize.Width,
                        height: CurrentMap.TileSize.Height
                    ),
                    name: $"Ammo{l}",
                    map: _maps[MapScene.City1]
                );
                i.Load(Globals.Content);
                ItemManager.AddItem(i);
            }


            i = new MedkitItem(
                rect: new Rectangle(
                    x: (int)Map.GetPosFromMap((10, 10), CurrentMap.TileSize).X,
                    y: (int)Map.GetPosFromMap((10, 10), CurrentMap.TileSize).Y,
                    width: CurrentMap.TileSize.Width,
                    height: CurrentMap.TileSize.Height
                ),
                name: $"Medkit",
                map: _maps[MapScene.City1]
            );
            i.Load(Globals.Content);
            ItemManager.AddItem(i);


            i = new AppleItem(
                rect: new Rectangle(
                    x: (int)Map.GetPosFromMap((15, 15), CurrentMap.TileSize).X,
                    y: (int)Map.GetPosFromMap((15, 15), CurrentMap.TileSize).Y,
                    width: CurrentMap.TileSize.Width,
                    height: CurrentMap.TileSize.Height
                ),
                name: $"Medkit",
                map: _maps[MapScene.City1]
            );
            i.Load(Globals.Content);
            ItemManager.AddItem(i);

            i = new GasMaskItem(
                rect: new Rectangle(
                    x: (int)Map.GetPosFromMap((15, 16), CurrentMap.TileSize).X,
                    y: (int)Map.GetPosFromMap((15, 16), CurrentMap.TileSize).Y,
                    width: CurrentMap.TileSize.Width,
                    height: CurrentMap.TileSize.Height
                ),
                name: $"Gas Mask",
                map: _maps[MapScene.City1]
            );
            i.Load(Globals.Content);
            ItemManager.AddItem(i);
        }
        
        if (scene == MapScene.Home)
        {
            i = new Gun(
                rect: new Rectangle(
                    x: (int)Map.GetPosFromMap((5, 20), CurrentMap.TileSize).X,
                    y: (int)Map.GetPosFromMap((5, 20), CurrentMap.TileSize).Y,
                    width: CurrentMap.TileSize.Width,
                    height: CurrentMap.TileSize.Height
                ),
                name: "First gun",
                map: _maps[scene],
                owner: null,
                dropped: true,
                infiniteAmmo: true
            );
            i.Load(Globals.Content);
            ItemManager.AddItem(i);

            i = new KeyItem(new Rectangle(120, 20, 16, 10), "Items/secret_key", name: "Secret key", doorToUnlock: "home_door_2", _maps[_mapScene], dropped: false);
            i.Load(Globals.Content);
            ItemManager.AddItem(i);

            i = new PhotoItem(
                rect: new Rectangle(
                    (int)Map.GetPosFromMap((5, 13), _maps[_mapScene].TileSize).X,
                    (int)Map.GetPosFromMap((5, 13), _maps[_mapScene].TileSize).Y,
                    CurrentMap.TileSize.Width,
                    CurrentMap.TileSize.Height
                ),
                src: "family/timmy",
                name: "Photo de timmy",
                map: _maps[scene],
                dropped: false
            );

            i.Load(Globals.Content);
            ItemManager.AddItem(i);
        }

        if (scene == MapScene.Labo)
        {
            i = new Gun(
                rect: new Rectangle(Vector2.Zero.ToPoint(), new Size(16).ToPoint()),
                name: "test",
                map: CurrentMap,
                owner: _player,
                infiniteAmmo: true
            );
            i.IsDropped = false;
            _player.Inventory.AddItem(i);
            ItemManager.AddAndLoad(i);

        }



        _itemsCreated[scene] = true;
    }


    public override void Load(ContentManager Content)
    {
        NpcManager.Init();
        CurrentMap.Load(Content);
        AudioManager.Init();
        Camera2D.Init(Globals.Graphics.GraphicsDevice.Viewport, CurrentMap);
        NotificationManager.Init();
        EventsManager.Instance = new EventsManager();
        Console.WriteLine($"Map is loaded: {CurrentMap.Loaded}");
        _player = new Player(
            rect: new Rectangle(
                (int)CurrentMap.DefaultMapPosition.X * CurrentMap.TileSize.Width,
                (int)CurrentMap.DefaultMapPosition.Y * CurrentMap.TileSize.Height,
                Sprite.GetSpriteSize().Width,
                Sprite.GetSpriteSize().Height
            ),
            src: "Player/idle",
            name: "Jason",
            speed: 5,
            health: 100,
            map: CurrentMap,
            debug: true
        );
        EntityManager.AddEntity(_player);

        Camera2D.Position = _player.Position;

        var pos = CurrentMap.GetAllWalkablesPosition();
        for (int i = 0; i < 5; i++)
        {
            var randomPos = pos[Utils.Random.Next(0, pos.Count)];
            var zombie = new Zombie(
                rect: new Rectangle(
                    randomPos.Item2 * CurrentMap.TileSize.Width,
                    randomPos.Item1 * CurrentMap.TileSize.Height,
                    Sprite.GetSpriteSize().Width,
                    Sprite.GetSpriteSize().Height
                ),
                src: "",
                speed: 2,
                health: 50,
                map: CurrentMap,
                debug: true
            );
            ZombieManager.AddZombie(zombie);
        }

        CreateItemsForScene(_mapScene);
        CreateEntitiesForScene(_mapScene);
        CreateInteractionsForMapScene(_mapScene);

        _player.Map = CurrentMap;
        _player.zombies = ZombieManager.Zombies;

        EntityManager.Player = _player;


        EntityManager.LoadEntities(Content);

        _inventoryWidget = new InventoryWidget(
            rect: new Rectangle(_InfoRect.X + 10, _InfoRect.Y + 10, _InfoRect.Width - 10, _InfoRect.Height),
            inventory: _player.Inventory
        );
        _userInfo = new UserInfoWidget(
            rect: _InfoRectRight,
            player: _player,
            debug: true
        );

        _inventoryWidget.Load(Content);
        _userInfo.Load(Content);
        QuestManager.CreateAllQuests(_player);

        Console.WriteLine("[+] GameScene loaded");

        AdminPanel.Instance = new AdminPanel();
        AdminPanel.Instance.Load(Content);

        Camera2D.SetTarget(_player);
    }

    public void TeleportPlayer(Vector2 newPosition)
    {
        _player.Position = newPosition;
    }

    public override void Update(GameTime gameTime)
    {
        CurrentMap.Update(gameTime);
        NotificationManager.Update(gameTime);
        TimerManager.Update(gameTime);
        Camera2D.Update(gameTime);
        QuestManager.Update(_player);
        CameraCinematicController.Update(gameTime);
        DialogManager.Instance.Update(gameTime);
        EventsManager.Instance.Update(gameTime);

        var currentScene = _mapScene;

        EntityManager.Update(gameTime, CurrentMap);

        InteractionObjectsManager.Update(gameTime, CurrentMap);

        _inventoryWidget.Update(gameTime);
        _userInfo.Update(gameTime);
        if (Camera2D.Target != null)
        {

        }
        else
        {
            if (InputManager.IsHolding(Keys.Right))
            {
                Camera2D.Move(new Vector2(5, 0), CurrentMap);
            }
            else if (InputManager.IsHolding(Keys.Left))
            {
                Camera2D.Move(new Vector2(-5, 0), CurrentMap);
            }
            if (InputManager.IsHolding(Keys.Up))
            {
                Camera2D.Move(new Vector2(0, -5), CurrentMap);
            }
            else if (InputManager.IsHolding(Keys.Down))
            {
                Camera2D.Move(new Vector2(0, 5), CurrentMap);
            }
        }
        if (InputManager.AreKeysPressedTogether(Keys.LeftControl, Keys.Up))
        {
            Camera2D.SetZoom(Camera2D.Zoom + 0.1f, CurrentMap);
            Console.WriteLine("zoom : " + Camera2D.Zoom);
        }
        else if (InputManager.AreKeysPressedTogether(Keys.LeftControl, Keys.Down))
        {
            Camera2D.SetZoom(Camera2D.Zoom - 0.1f, CurrentMap);
        }

        if (InputManager.IsPressed(Keys.C))
        {
            Camera2D.SetTarget(Camera2D.Target == null ? _player : null);
        }


        if (InputManager.IsPressed(Keys.Escape))
        {
            PauseScene pause = new PauseScene(SceneState.Pause, rect: Globals.FullScreenRect, OnClose: () =>
            {
                SceneManager.ChangeScreen(SceneState.Game);
                SceneManager.RemoveScene(SceneState.Pause);
            });
            pause.Load(Globals.Content);

            SceneManager.AddScene(SceneState.Pause, pause);
            SceneManager.ChangeScreen(SceneState.Pause);
        }

        if (InputManager.IsPressed(Keys.H))
        {
            Npc npc = (Npc)EntityManager.GetFirst(e => e is Npc);
            if (npc != null)
            {
                var mousePos = Camera2D.ScreenToMap(InputManager.GetMousePosition());
                path = CurrentMap.FindPath(npc.Position, mousePos);
                path.Reverse();
                npc.MovePath = path;
                PathDrawer.AddPath(path);
            }
        }

        AdminPanel.Instance.Update(gameTime);
        if (!AdminPanel.Instance.Show)
        {
            if (InputManager.AreKeysPressedTogether(Keys.LeftControl, Keys.A))
            {
                AdminPanel.Instance.Show = true;
            }
        }

        if (InputManager.IsHolding(Keys.LeftControl) && InputManager.LeftClicked)
        {
            var map = CurrentMap;
            var matrix = Camera2D.GetViewMatrix() * map.ScaleMatrix;
            var p = Vector2.Transform(InputManager.GetMousePosition(), Matrix.Invert(matrix));
            TeleportPlayer(p);
        }
    }


    public override void Draw(SpriteBatch _spriteBatch)
    {
        var graphicsDevice = Globals.Graphics.GraphicsDevice;

        var oldViewport = graphicsDevice.Viewport;
        // On crée un rectangle centré autour de ce point, taille 200x200 pixels
        Rectangle renderRect = new(
            0,
            0,
            (int)(Camera2D.CameraLogicalSize.Width * CurrentMap.ScaleMatrix.M11),
            (int)(Camera2D.CameraLogicalSize.Height * CurrentMap.ScaleMatrix.M22)
        );
        graphicsDevice.Viewport = new Viewport(renderRect.X, renderRect.Y, renderRect.Width, renderRect.Height);
        _spriteBatch.End(); // Termine un éventuel Begin() précédent




        // Affichage de la map
        // Commence le batch avec transformation caméra + scale

        var matrix = Camera2D.GetViewMatrix() * CurrentMap.ScaleMatrix;
        var context = new SpriteBatchState(
            transformMatrix: matrix,
            rasterizerState: new RasterizerState { ScissorTestEnable = true },
            samplerState: SamplerState.PointClamp
        );
        SpriteBatchContext.Push(context); // Push le batch
        SpriteBatchContext.ApplyToContext(_spriteBatch, SpriteBatchContext.Top); // lance le back enpilé


        //Dessine la map
        CurrentMap.Draw(_spriteBatch);
        foreach (var layer in CurrentMap.TiledMap.Layers)
        {
            if (layer.Name != "forground")
            {
                SpriteBatchContext.Restart(_spriteBatch);
                CurrentMap.DrawLayer(_spriteBatch, layer.Name);
            }
        }

        SpriteBatchContext.Restart(_spriteBatch);

        // Dessine tout ce qui doit suivre la caméra (joueur, items, ennemis, etc.)
        EntityManager.Draw(_spriteBatch, CurrentMap);

        // foreach (var obj in InteractionObjects[_mapScene])
        // {
        //     obj.Draw(_spriteBatch);
        // }
        InteractionObjectsManager.Draw(_spriteBatch, CurrentMap);
        PathDrawer.Draw(_spriteBatch);



        SpriteBatchContext.Restart(_spriteBatch);


        CurrentMap.DrawLayer(_spriteBatch, "forground");

        _spriteBatch.End(); // Termine ce batch transformé
        SpriteBatchContext.Pop(); // depile le batch crée

        graphicsDevice.Viewport = oldViewport;

        // Remet le rasterizer par défaut pour la suite
        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

        // Démarre un nouveau batch SANS transformation pour l’UI
        SpriteBatchContext.ApplyToContext(_spriteBatch, SpriteBatchContext.Top);

        Shape.DrawRectangle(_spriteBatch, renderRect, Color.Purple);
        _player.DrawPosition(_spriteBatch);



        _inventoryWidget.Draw(_spriteBatch);
        if (QuestManager.IsPlayingQuest)
        {
            Text.Write(_spriteBatch, $"[{QuestManager.CurrentQuest.Title}]", new Vector2(_InfoRect.X + 500, _InfoRect.Y), Color.White);
            Text.Write(_spriteBatch, $"Goal: [{QuestManager.CurrentQuest.Description}]", new Vector2(_InfoRect.X + 500, _InfoRect.Y + 50), Color.White);
        }
        _userInfo.Draw(_spriteBatch);
        DialogManager.Instance.Draw(_spriteBatch);

        NotificationManager.Draw(_spriteBatch);

        if (AdminPanel.Instance.Show) AdminPanel.Instance.Draw(_spriteBatch);

        if (_debug)
            DrawDebug(_spriteBatch);
    }
}