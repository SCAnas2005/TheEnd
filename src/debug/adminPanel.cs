
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheEnd;

public enum AdminPanelState
{
    SelectingEntities,
    AddingEntities,
    EntityProperties,
    PlaceEntityWithMouse
}

public class AdminPanel : StatefulWidget
{
    public static AdminPanel Instance;
    private Widget _container;
    private Widget _controlContainer;
    private Widget _entityPropertiesContainer;
    private Widget _mapsContainer;
    private float _updateTimer = 0f;
    private float UPDATE_TIME = 500f;

    public bool Show { get; set; }

    private Entity _selectedEntity = null;
    private ScrollViewWidget scrollWidget;
    private int _saveScroll = 0;


    private bool addEntity = false;
    private AdminPanelState _state;
    private Type entityToAdd = null;

    public AdminPanel()
    {
        Show = false;
        _state = AdminPanelState.SelectingEntities;
        Build();
    }

    public override void Build()
    {
        List<Widget> entitiesWidgets = [];
        foreach (var e in EntityManager.Entities)
        {
            if (!e.KillMe)
            {
                var w = new ContainerWidget(
                    size: new Size(60),
                    widgets: [
                        new ContainerWidget(
                            size: new Size(50),
                            border: _selectedEntity == e ? new Border(all: 2, color: Color.Purple) : new Border(all: 1, color: Color.Black),
                            widgets: [
                                new ButtonWidget(
                                    size: new Size(50), background: null, backgroundColor: Color.DarkGray, text: new TextWidget(e.ToString()),
                                    OnClick: () => { _selectedEntity = e; addEntity = false; SetState(); }
                                ),
                            ]
                        ),

                        new SizedBox(new Size(10))
                    ]
                );
                entitiesWidgets.Add(w);
            }
        }

        var addButton = new ContainerWidget(
            size: new Size(60),
            widgets: [
                new ContainerWidget(
                    size: new Size(50),
                    border: new Border(all: 1, color: Color.Black),
                    widgets: [
                        new ButtonWidget(
                            size: new Size(50), background: null, backgroundColor: Color.DarkGray, text: new TextWidget("Add"),
                            OnClick: () => { _selectedEntity = null; addEntity = true; _state = AdminPanelState.AddingEntities; }
                        ),
                    ]
                ),

                new SizedBox(new Size(10))
            ]
        );
        entitiesWidgets.Add(addButton);

        scrollWidget = new ScrollViewWidget(
            scrollPosition: _saveScroll,
            child: new ContainerWidget(
                mainAxisAlignment: MainAxisAlignment.Start,
                crossAxisAlignment: CrossAxisAlignment.Start,
                size: new Size(900, 300),
                wrap: true,
                widgets: [.. entitiesWidgets],
                debug: true
            )
        );
        _container = new ContainerWidget(
            rect: new Rectangle(200, 500, 1000, 400),
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Start,
            backgroundColor: new Color(0, 0, 0, 150),
            alignItem: Align.Vertical,
            padding: new Padding(10),
            widgets: [
                new ContainerWidget(
                    size: new Size(900, 30),
                    mainAxisAlignment: MainAxisAlignment.SpaceBetween,
                    widgets: [
                        new TextWidget($"Entities (All:{EntityManager.Count}, zombies:{ZombieManager.Count}, items:{ItemManager.Count}), npcs: {NpcManager.Count}"),
                        new ButtonWidget(size: new Size(50, 30), background: null, text: new TextWidget("Close"), OnClick: () => {Console.WriteLine("Close button pressed"); Show = false; }, debug:true)
                    ]
                ),
                scrollWidget,
                new TextButtonWidget(
                    size: new Size(50,30),
                    text: new TextWidget("Clear", color: Color.Red),
                    OnClick: () => {
                        EntityManager.RemoveAllExcept([typeof(Player)]);
                        SetState();
                    },
                    backgroundColor: new Color(171,171,171)
                ),
            ]
        );


        List<Widget> actionsWidget = [];

        if (_state == AdminPanelState.AddingEntities)
        {
            actionsWidget.Add(new TextWidget("Classes"));
            List<Type> types = EntityManager.GetAllEntityTypes(excludes:[typeof(Player)]);
            foreach (var type in types)
            {
                actionsWidget.Add(
                    new ContainerWidget(
                        size: new Size(300, 30),
                        border: new Border(all: 1, Color.LightGray),
                        backgroundColor: Color.LightGray,
                        widgets: [
                            new TextButtonWidget(
                                size: new Size(300, 30),
                                text: new TextWidget($"{type.Name}"),
                                backgroundColor: new Color(0,0,0,50),
                                OnClick: () => { EntityManager.RemoveEntity(_selectedEntity);
                                    _selectedEntity = null;
                                    entityToAdd = type;
                                    _state = AdminPanelState.PlaceEntityWithMouse;
                                    Show = false;
                                    // SetState();
                                }
                            ),
                        ]
                    )
                );
            }
        }
        else if (_state == AdminPanelState.SelectingEntities)
        {
            if (_selectedEntity != null)
            {
                actionsWidget.Add(
                    new ContainerWidget(
                        size: new Size(300, 30),
                        border: new Border(all: 1, Color.LightGray),
                        backgroundColor: Color.LightGray,

                        widgets: [
                            new TextButtonWidget(
                                size: new Size(300, 30),
                                text: new TextWidget("Supprimer"),
                                backgroundColor: new Color(0,0,0,50),
                                OnClick: () => {
                                    EntityManager.RemoveEntity(_selectedEntity); _selectedEntity = null; _state = AdminPanelState.SelectingEntities;  SetState();
                                }
                            ),
                        ]
                    )
                );
            }
        }

        _controlContainer = new ContainerWidget(
            rect: new Rectangle((_container.Position + new Vector2(_container.Size.Width + 50, 50)).ToPoint(), new Size(300, 500).ToPoint()),
            alignItem: Align.Vertical,
            backgroundColor: new Color(0, 0, 0, 150),
            widgets: [  .. actionsWidget, _selectedEntity != null ? GetInfoFromEntity(_selectedEntity) : new SizedBox()]
        );




        Dictionary<string, Type> properties = entityToAdd != null ? EntityManager.GetPublicPropertiesOfType(entityToAdd): [];
        List<Widget> propertiesWidget = [];
        foreach (var (name, type) in properties)
        {
            var w = new ContainerWidget(
                widgets: [
                    new TextWidget($"{name}({type.Name})")
                ]
            );
            propertiesWidget.Add(w);
        }
        List<Widget> wList = entityToAdd != null ? [
            new TextWidget($"{entityToAdd.Name}", color: Color.White),
            new SizedBox(height: 20),
            new ContainerWidget(
                crossAxisAlignment: CrossAxisAlignment.Start,
                alignItem: Align.Vertical,
                size: new Size(500,800),
                widgets: [
                    new TextWidget("Entity", color: Color.White),
                    new ContainerWidget(
                        alignItem: Align.Vertical,
                        widgets: [.. propertiesWidget]
                    ),
                ]
            ),
        ] : [];
        _entityPropertiesContainer = new ContainerWidget(
            rect: new Rectangle(500,200,500,800),
            alignItem: Align.Vertical,
            backgroundColor: new Color(15,15,15),
            widgets: [.. wList]
        );

        List<Widget> _mapButtonsWidgets = [];
        foreach (var (mapscene, map) in ((GameScene)SceneManager.GetScene(SceneState.Game)).Maps)
        {
            _mapButtonsWidgets.Add(
                new TextButtonWidget(
                    size: new Size(150,30),
                    text: new TextWidget($"{mapscene}"),
                    backgroundColor: Color.LightGray,
                    OnClick: () =>
                    {
                        Console.WriteLine($"map : {mapscene}");
                    }
                )
            );
        }

        _mapsContainer = new ContainerWidget(
            rect: new Rectangle((_controlContainer.Position + new Vector2(_controlContainer.Size.Width + 50, 50)).ToPoint(), new Size(150, 300).ToPoint()),
            alignItem: Align.Vertical,
            mainAxisAlignment: MainAxisAlignment.Start,
            crossAxisAlignment: CrossAxisAlignment.Start,       
            backgroundColor: Color.Gray,
            widgets: [
                new TextWidget("maps"),
                new TextWidget("Teleport to"),
                new ContainerWidget(
                    alignItem: Align.Vertical,
                    mainAxisAlignment: MainAxisAlignment.Start,
                    crossAxisAlignment: CrossAxisAlignment.Start,
                    widgets: [
                        .. _mapButtonsWidgets,
                        new SizedBox(height: 50),
                        new TextButtonWidget(size: new Size(150,20), text: new TextWidget("Reinit"), backgroundColor: Color.SlateGray)
                    ]
                ),
            ]
        );


        if (!_loaded)
        {
            Load(Globals.Content);
        }
    }


    public Widget GetInfoFromEntity<T>(T e) where T : Entity
    {
        List<Widget> l = [];
        l.AddRange([
            new TextWidget($"{e.GetType().Name}"),
            new TextWidget($"Position : {e.Position}"),
            new TextWidget($"Size: {e.Size}"),
            new ContainerWidget(
                widgets: [
                    new TextWidget("Texture: "),
                    new ImageWidget(size: new Size(30), texture: e.Texture)
                ]
            ),
            new TextWidget($"Killme: {e.KillMe}"),
            new TextWidget($"IsSolid: {e.IsSolid}"),
            new TextWidget($"Map: {e.Map.Scene}"),
            new TextWidget($"zIndex: {e.zIndex}"),
        ]);
        if (e is Item)
        {

        }
        ContainerWidget w = new(
            size: new Size(300, 600),
            alignItem: Align.Vertical,
            widgets: [.. l]
        );

        return w;
    }

    public override void Load(ContentManager Content)
    {
        _container.Load(Content);
        _controlContainer.Load(Content);
        _entityPropertiesContainer.Load(Content);
        _mapsContainer.Load(Globals.Content);
        _loaded = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (Show)
        {
            base.Update(gameTime);
            _updateTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_updateTimer >= UPDATE_TIME)
            {
                SetState();
                _updateTimer = 0f;
            }
            _container.Update(gameTime);
            _controlContainer.Update(gameTime);
            _mapsContainer.Update(gameTime);
            if (_state == AdminPanelState.EntityProperties)
            {
                _entityPropertiesContainer.Update(gameTime);
            }
            _saveScroll = scrollWidget.ScrollPosition;

            if (InputManager.IsPressed(Keys.Delete) && _selectedEntity != null && !_selectedEntity.KillMe)
            {
                EntityManager.RemoveEntity(_selectedEntity); _selectedEntity = null; SetState();
            }
        }
        else
        {
            if (_state == AdminPanelState.PlaceEntityWithMouse)
            {
                if (InputManager.LeftClicked)
                {
                    var map = EntityManager.Player.Map;
                    var matrix = Camera2D.GetViewMatrix() * map.ScaleMatrix;
                    var p = Vector2.Transform(InputManager.GetMousePosition(), Matrix.Invert(matrix));
                    AddNewEntity(entityToAdd, p, EntityManager.Player.Map);
                    _state = AdminPanelState.SelectingEntities;
                    Show = true;
                    entityToAdd = null;
                }
            }
        }

    }

    public void AddNewEntity(Type t, Vector2 position, Map map)
    {
        if (t == null) return;
        Console.WriteLine("Add new entity");
        Entity e = null;
        if (typeof(Item).IsAssignableFrom(t))
        {
            e = ItemManager.CreateBasicFromType(t, position, map);
            ItemManager.AddItem((Item)e);
        }
        else if (typeof(Npc).IsAssignableFrom(t))
        {
            e = NpcManager.CreateBasicFromType(t, position, map);
            NpcManager.AddNpc((Npc)e);
        }
        else if (typeof(Zombies).IsAssignableFrom(t))
        {
            e = ZombieManager.CreateBasicFromType(t, position, map);
            ZombieManager.AddZombie((Zombies)e);
        }
        else
        {
            e = EntityManager.CreateBasicFromType(t, position, map);
            EntityManager.AddEntity(e);
            EntityManager.GetPublicPropertiesOfType(t);
        }
        Console.WriteLine("entity : " + e + " added");
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (Show)
        {
            base.Draw(_spriteBatch);
            _container.Draw(_spriteBatch);
            _controlContainer.Draw(_spriteBatch);
            _mapsContainer.Draw(_spriteBatch);
            if (_state == AdminPanelState.EntityProperties)
            {
                _entityPropertiesContainer.Draw(_spriteBatch);
            }
        }
    }
}

