
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

public class Map
{
    public Rectangle _rect;
    public string Name;
    private TiledMap _map;
    private TiledMapRenderer _mapRenderer;
    private Size _tileSize;
    private float _scale;
    private Matrix _scaleMatrix;

    private string _src;

    public Size TileSize {get{return _tileSize;}}
    public Size Size {get{return new Size(_rect.Width, _rect.Height);} }
    public Rectangle Rect {get{return _rect;}}
    public Vector2 Position {get{ return new Vector2(_rect.X, _rect.Y); } set{ _rect.X = (int)value.X;  _rect.Y = (int)value.Y; }}
    public float Scale { get { return _scale; } }
    public Matrix ScaleMatrix {get{return _scaleMatrix;}}

    public TiledMap TiledMap {get{return _map;}}

    public MapScene Scene;
    public bool debug;
    public bool Loaded;



    public Map(Rectangle rect, string src, string name, MapScene scene, bool debug = false)
    {
        _rect = rect;
        _src = src;
        this.debug = debug;
        Name = name;
        Scene = scene;
        Loaded = false;
    }

    public void InitPosition(int x, int y)
    {
        Position = new Vector2(x, y);
    }

    public static (int, int) GetMapPosFromVector2(Vector2 pos, Size tileSize)
    {
        return ((int)pos.Y / tileSize.Height, (int)pos.X / tileSize.Width);
    }
    
    public static Vector2 GetPosFromMap((int, int) pos, Size tileSize)
    {
        return new Vector2(pos.Item2*tileSize.Width, pos.Item1*tileSize.Height);
    }


    public bool IsInMap((int l, int c)p)
    {
        return p.l >= 0 && p.l < _map.Width && p.c >= 0 && p.c < _map.Height;
    }

    public void ToggleDebug()
    {
        debug = !debug;
    }

    public void CreateAllInteractions()
    {
        bool hasInteractionLayer = TiledMap.Layers.Any(layer => layer.Name == "Interaction");
        if (!hasInteractionLayer) return;

        var interactionLayer = TiledMap.GetLayer<TiledMapObjectLayer>("Interaction");
    }

    public void UpdateMapRenderer()
    {
        _mapRenderer = new TiledMapRenderer(Globals.Graphics.GraphicsDevice);
        _mapRenderer.LoadMap(_map);
    }


    public void Load(ContentManager Content)
    {
        _map = Content.Load<TiledMap>(_src);
        float scaleX = Globals.ScreenSize.Width / (float)_map.WidthInPixels;
        float scaleY = Globals.ScreenSize.Height*4/5 / (float)_map.HeightInPixels;
        _scale = Math.Min(scaleX, scaleY);
        Globals.TileScale = _scale;
        _scaleMatrix = Matrix.CreateScale(_scale);

        _rect.Width = _map.Width*_map.TileWidth;
        _rect.Height = _map.Height*_map.TileHeight;

        _mapRenderer = new TiledMapRenderer(Globals.Graphics.GraphicsDevice);
        _mapRenderer.LoadMap(_map);


        _tileSize = new Size(_map.TileWidth, _map.TileHeight);
        Globals.MapTileSize = _tileSize;
        Loaded = true;
    }

    public void Update(GameTime gameTime)
    {
        if (InputManager.IsPressed(Keys.M))
        {
            ToggleDebug();
        }
    }
    
    public List<Node> FindPath(TiledMap map, string obstacleLayerName, Vector2 start, Vector2 end)
    {
        var openSet = new List<Node>();
        var closedSet = new HashSet<Node>();

        var startNode = new Node((int)start.X, (int)start.Y);
        var endNode = new Node((int)end.X, (int)end.Y);
        openSet.Add(startNode);

        var layer = map.GetLayer<TiledMapTileLayer>(obstacleLayerName);

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(n => n.F).First();

            if (current.X == end.X && current.Y == end.Y)
            {
                var path = new List<Node>();
                while (current != null)
                {
                    path.Add(current);
                    current = current.Parent;
                }
                path.Reverse();
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current, map, layer))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int tentativeG = current.G + 1;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeG >= neighbor.G)
                    continue;

                neighbor.Parent = current;
                neighbor.G = tentativeG;
                neighbor.H = Math.Abs(neighbor.X - (int)end.X) + Math.Abs(neighbor.Y - (int)end.Y);
            }
        }

        return null; // Aucun chemin trouvé
    }

    public List<Node> GetNeighbors(Node node, TiledMap map, TiledMapTileLayer obstacleLayer)
    {
        var neighbors = new List<Node>();
        int[,] dirs = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };

        for (int i = 0; i < 4; i++)
        {
            int nx = node.X + dirs[i, 0];
            int ny = node.Y + dirs[i, 1];

            if (nx >= 0 && ny >= 0 && nx < map.Width && ny < map.Height)
            {
                TiledMapTile? tile;
                if (!obstacleLayer.TryGetTile((ushort)nx, (ushort)ny, out tile) || tile.Value.GlobalIdentifier == 0)
                {
                    neighbors.Add(new Node(nx, ny));
                }
            }
        }

        return neighbors;
    }



    public void DrawCollisions(SpriteBatch _spriteBatch)
    {
        var collisionLayer = TiledMap.GetLayer<TiledMapTileLayer>("obstacles");
        for (ushort l = 0; l < collisionLayer.Height; l++)
        {
            for (ushort c = 0; c < collisionLayer.Width; c++)
            {
                var tile = collisionLayer.GetTile(c, l);

                if (tile.GlobalIdentifier != 0)
                {
                    Shape.DrawRectangle(_spriteBatch, new Rectangle(_rect.X + c * _tileSize.Width, _rect.Y + l * _tileSize.Height, _tileSize.Width, _tileSize.Height), Color.Red);
                }
            }
        }
    }


    public List<(int, int)> GetAllWalkablesPosition()
    {
        List<(int, int)> walkableTiles = new List<(int, int)>();

        // Récupère la couche "obstacles"
        var obstacleLayer = TiledMap.GetLayer<TiledMapTileLayer>("obstacles");

        for (int x = 0; x < obstacleLayer.Width; x++)
        {
            for (int y = 0; y < obstacleLayer.Height; y++)
            {
                var tile = obstacleLayer.GetTile((ushort)x, (ushort)y);
                if (tile.GlobalIdentifier == 0)
                {
                    walkableTiles.Add((y, x));
                }
            }
        }

        return walkableTiles;
    }


    public void Draw(SpriteBatch _spriteBatch)
    {
        _mapRenderer.Draw(_scaleMatrix);
        if (debug)
        {
            DrawCollisions(_spriteBatch);
            Shape.DrawRectangle(_spriteBatch, Rect, Color.Blue);
        }
    }
}