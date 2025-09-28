
using System;
using System.Collections.Generic;
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
    public float Zoom;

    private string _src;

    public Size TileSize {get{return _tileSize;}}
    public Size Size {get{return new Size(_rect.Width, _rect.Height);} }
    public Rectangle Rect { get { return _rect; } }
    public Vector2 Position {get{ return new Vector2(_rect.X, _rect.Y); } set{ _rect.X = (int)value.X;  _rect.Y = (int)value.Y; }}
    public float Scale { get { return _scale; } }
    public Matrix ScaleMatrix {get{return _scaleMatrix;}}
    public Size ScaledSize {get{ return new Size((int)(_rect.Width * ScaleMatrix.M11), (int)(_rect.Height*ScaleMatrix.M11)); }}

    public TiledMap TiledMap {get{return _map;}}

    public MapScene Scene;
    public bool debug;
    public bool Loaded;

    public int Width { get { return _rect.Width; }}
    public int Height {get{ return _rect.Height; }}


    private List<TiledMapLayer> _layers;

    private List<Rectangle> _zombieSpawnZones;
    public List<Rectangle> ZombieSpawnZones { get => _zombieSpawnZones; }



    public Map(Rectangle rect, string src, string name, MapScene scene, float zoom = 1f, bool debug = false)
    {
        _rect = rect;
        _src = src;
        Zoom = zoom;
        this.debug = debug;
        Name = name;
        Scene = scene;
        Loaded = false;
        _zombieSpawnZones = [];
    }

    public void InitPosition(int x, int y)
    {
        Position = new Vector2(x, y);
    }

    
    public static (int Row, int Col) GetMapPosFromVector2(Vector2 screenPos, Size tileSize)
    {
        // Vector2 worldPos = Camera2D.ScreenToWorld(screenPos);
        // int row = (int)(worldPos.Y / tileSize.Height);
        // int col = (int)(worldPos.X / tileSize.Width);
        int row = (int)(screenPos.Y / tileSize.Height);
        int col = (int)(screenPos.X / tileSize.Width);
        return (row, col);
    }

    public static Vector2 GetPosFromMap((int, int) pos, Size tileSize)
    {
        return new Vector2(pos.Item2 * tileSize.Width, pos.Item1 * tileSize.Height);
    }


    public bool IsInMap((int l, int c) p)
    {
        return p.l >= 0 && p.l < _map.Width && p.c >= 0 && p.c < _map.Height;
    }

    public void ToggleDebug()
    {
        debug = !debug;
    }

    public TiledMapObjectLayer GetAllInteractions()
    {
        var interactionLayer = TiledMap.GetLayer<TiledMapObjectLayer>("Interaction");
        return interactionLayer;
    }

    public TiledMapObjectLayer GetAllEntities()
    {
        var entitiesLayer = TiledMap.GetLayer<TiledMapObjectLayer>("Entities");
        return entitiesLayer;
    }

    public void UpdateMapRenderer()
    {
        _mapRenderer = new TiledMapRenderer(Globals.Graphics.GraphicsDevice);
        _mapRenderer.LoadMap(_map);
    }

    public T GetLayer<T>(string name) where T : TiledMapLayer
    {
        return TiledMap.GetLayer<T>(name);
    }



    public void Load(ContentManager Content)
    {
        _map = Content.Load<TiledMap>(_src);
        float scaleX = Globals.ScreenSize.Width / (float)_map.WidthInPixels;
        float scaleY = Globals.ScreenSize.Height / (float)_map.HeightInPixels;
        _scale = 1f;//Math.Min(scaleX, scaleY);
        Globals.TileScale = _scale;
        _scaleMatrix = Matrix.Identity;//Matrix.CreateScale(_scale);

        _rect.Width = _map.Width * _map.TileWidth;
        _rect.Height = _map.Height * _map.TileHeight;

        _mapRenderer = new TiledMapRenderer(Globals.Graphics.GraphicsDevice);
        _mapRenderer.LoadMap(_map);

        _layers = [.. TiledMap.Layers];

        _tileSize = new Size(_map.TileWidth, _map.TileHeight);
        Globals.MapTileSize = _tileSize;

        var zombieZonesLayer = GetLayer<TiledMapObjectLayer>("ZombieSpawnZones");
        if (zombieZonesLayer != null)
        {
            foreach (var o in zombieZonesLayer.Objects)
            {
                ZombieSpawnZones.Add(new Rectangle(o.Position.ToPoint(), new Size((int)o.Size.Width, (int)o.Size.Height).ToPoint()));
            }
        }

        Loaded = true;
    }

    public bool InZombieSpawningZone(Rectangle rect)
    {
        foreach (var z in ZombieSpawnZones)
        {
            if (rect.Intersects(z)) return true;
        }
        return false;
    }

    public void Update(GameTime gameTime)
    {
        _mapRenderer.Update(gameTime);
        if (InputManager.IsPressed(Keys.M))
        {
            ToggleDebug();
        }
    }

    private float GetHeuristic(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private List<Vector2> ReconstructPath(Node node)
    {
        List<Vector2> path = new();
        while (node != null)
        {
            path.Add(new Vector2(
                node.Position.X * TileSize.Width + TileSize.Width / 2f,
                node.Position.Y * TileSize.Height + TileSize.Height / 2f
            ));
            node = node.Parent;
        }
        return path;
    }

    public List<Vector2> FindPath(Vector2 s, Vector2 e, Size max = null)
    {
        Point start = new Point((int)s.X / TileSize.Width, (int)s.Y / TileSize.Height), end = new Point((int)e.X / TileSize.Width, (int)e.Y / TileSize.Height);
        var openList = new List<Node>();
        var closedList = new HashSet<Point>();

        Size Max = max ?? new Size(_map.Width, _map.Height);

        Node startNode = new Node(start, 0, GetHeuristic(start, end));
        openList.Add(startNode);

        var maxCase = Max.Width * Max.Height;

        while (openList.Count > 0 && closedList.Count < maxCase)
        {
            openList.Sort((a, b) => a.F.CompareTo(b.F));
            Node current = openList.First();
            openList.RemoveAt(0);

            if (current.Position == end)
            {
                return ReconstructPath(current);
            }

            closedList.Add(current.Position);

            foreach (Point neighbor in GetNeighbors(current.Position))
            {
                if (closedList.Contains(neighbor) || !IsWalkablePoint(neighbor)) continue;

                float tentativeG = current.G + Vector2.Distance(current.Position.ToVector2(), neighbor.ToVector2());
                Node neighborNode = openList.FirstOrDefault(n => n.Position == neighbor);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighbor, tentativeG, GetHeuristic(neighbor, end), current);
                    openList.Add(neighborNode);
                }
                else if (tentativeG < neighborNode.G)
                {
                    neighborNode.G = tentativeG;
                    neighborNode.Parent = current;
                }
            }
        }

        return new List<Vector2>();
    }

    

    public List<Point> GetNeighbors(Point p)
    {
        var neighbors = new List<Point>();

        Point[] directions = new Point[]
        {
            new Point(0, -1), // Haut
            new Point(1, -1), // Haut droite
            new Point(-1, -1), // Haut gauche
            new Point(1, 0),  // Droite
            new Point(0, 1),  // Bas
            new Point(1, 1), // bas droite
            new Point(-1, 1), // Bas gauche
            new Point(-1, 0), // Gauche
        };

        foreach (var dir in directions)
        {
            Point next = new Point(p.X + dir.X, p.Y + dir.Y);
            if (Math.Abs(dir.X) == 1 && Math.Abs(dir.Y) == 1) // Si next est une case diagonale, on vérifie les cotés adjacents
            {
                Point check1 = new Point(p.X + dir.X, p.Y);
                Point check2 = new Point(p.X, p.Y + dir.Y);

                if (!IsWalkablePoint(check1) || !IsWalkablePoint(check2)) continue;
            }
            if (InBounds(next) && IsWalkablePoint(next))
            {
                neighbors.Add(next);
            }
        }
        return neighbors;
    }

    public bool InBounds(Point point)
    {
        return point.X >= 0 && point.Y >= 0 && point.X < TiledMap.Width && point.Y < TiledMap.Height;
    }

    

    public bool InBounds(Vector2 position)
    {
        (var line, var col) = GetMapPosFromVector2(position, TileSize);
        Point p = new Point(line, col);
        return InBounds(p);
    }

    public bool IsInGrass(Vector2 position)
    {
        var tileLayer = TiledMap.GetLayer<TiledMapTileLayer>("ground");
        (int row, int col) = GetMapPosFromVector2(position, TileSize);

        if (tileLayer.TryGetTile((ushort)col, (ushort)row, out var tile))
        {
            int gid = tile.Value.GlobalIdentifier;

            // Trouver le bon tileset via TilesetReferences
            foreach (var tileset in TiledMap.Tilesets)
            {
                foreach (var t in tileset.Tiles)
                {
                    if (t.LocalTileIdentifier == gid)
                    {
                        if (t.Properties.ContainsKey("type") && t.Properties["type"] == "grass")
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    public void DrawCollisions(SpriteBatch _spriteBatch)
    {
        var collisionLayer = TiledMap.GetLayer<TiledMapTileLayer>("obstacles");
        if (collisionLayer == null) return;
        for (ushort l = 0; l < collisionLayer.Height; l++)
        {
            for (ushort c = 0; c < collisionLayer.Width; c++)
            {
                var tile = collisionLayer.GetTile(c, l);

                if (tile.GlobalIdentifier != 0)
                {
                    // Shape.DrawRectangle(_spriteBatch, new Rectangle((int)-WorldPosition.X + c * _tileSize.Width, (int)-WorldPosition.Y + l * _tileSize.Height, _tileSize.Width, _tileSize.Height), Color.Red);
                    Shape.DrawRectangle(_spriteBatch, new Rectangle(c * _tileSize.Width, l * _tileSize.Height, _tileSize.Width, _tileSize.Height), Color.Red);
                }
            }
        }
    }


    public bool IsWalkablePoint(Point p)
    {
        if (!InBounds(p)) return false;
        var layerName = TiledMap.GetLayer<TiledMapTileLayer>("obstacles") != null ? "obstacles" : "ground";
        var layer = TiledMap.GetLayer<TiledMapTileLayer>(layerName);
        // var solidEntities = ((GameScene)SceneManager.GetScene(SceneState.Game))._entities.Where(e => e.IsSolid);
        var solidEntities = EntityManager.GetEntitiesWhere((e) => e.IsSolid);

        if (layer == null) return false;
        if (layerName == "obstacles")
        {
            var tile = layer.GetTile((ushort)p.X, (ushort)p.Y);
            if (tile.GlobalIdentifier != 0) return false;
        }

        foreach (var e in solidEntities)
        {
            if (GetMapPosFromVector2(e.Position, TileSize) == (p.Y, p.X)) return false;
        }
        return true;
    }


    public List<(int row, int col)> GetAllWalkablesPosition()
    {
        var walkableTiles = new List<(int, int)>();
        // var blackList = ((GameScene)SceneManager.GetScene(SceneState.Game))._entities.Where(e => e.IsSolid).ToList(); // blacklist for entities position not finished
        var blackList = EntityManager.GetEntitiesWhere((e) => e.IsSolid);

        // Récupère le bon calque (priorité à "obstacles", sinon "ground")
        var layerName = TiledMap.GetLayer<TiledMapTileLayer>("obstacles") != null ? "obstacles" : "ground";
        var layer = TiledMap.GetLayer<TiledMapTileLayer>(layerName);


        if (layer == null)
            return walkableTiles; // Aucun calque trouvé

        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                var tile = layer.GetTile((ushort)x, (ushort)y);

                // Si c'est "obstacles", on ne garde que les cases vides
                if (layerName == "obstacles")
                {
                    if (tile.GlobalIdentifier == 0)
                        walkableTiles.Add((y, x));
                }
                else
                {
                    // Dans les autres cas (ground), tout est considéré comme marchable
                    walkableTiles.Add((y, x));
                }
            }
        }
        return walkableTiles;
    }

    public List<(int row, int col)> GetWalkablesPositionFromPosition(Vector2 position, Size area)
    {
        Vector2 startPos = position - (area / 2).ToVector2();

        (int startRow, int startCol) = GetMapPosFromVector2(startPos, TileSize);
        
        var walkableTiles = new List<(int, int)>();
        // var blackList = ((GameScene)SceneManager.GetScene(SceneState.Game))._entities.Where(e => e.IsSolid).ToList(); // blacklist for entities position not finished
        var blackList = EntityManager.GetEntitiesWhere((e) => e.IsSolid);

        // Récupère le bon calque (priorité à "obstacles", sinon "ground")
        var layerName = TiledMap.GetLayer<TiledMapTileLayer>("obstacles") != null ? "obstacles" : "ground";
        var layer = TiledMap.GetLayer<TiledMapTileLayer>(layerName);

        if (layer == null)
            return walkableTiles; // Aucun calque trouvé

        for (int x = startCol; x < startCol + area.Width; x++)
        {
            for (int y = startRow; y < startRow + area.Height; y++)
            {
                if (x < 0 || x >= layer.Width || y < 0 || y >= layer.Height)
                    continue;
                if (blackList.Any(e => {
                    var (row, col) = GetMapPosFromVector2(e.Position, TileSize);
                    return row == y && col == x;
                }))
                    continue;
                var tile = layer.GetTile((ushort)x, (ushort)y);

                // Si c'est "obstacles", on ne garde que les cases vides
                if (layerName == "obstacles")
                {
                    if (tile.GlobalIdentifier == 0)
                        walkableTiles.Add((y, x));
                }
                else
                {
                    // Dans les autres cas (ground), tout est considéré comme marchable
                    walkableTiles.Add((y, x));
                }
            }
        }
        return walkableTiles;
    }


    public void DrawLayer(SpriteBatch _spriteBatch, string layerName)
    {
        var layer = TiledMap.GetLayer(layerName);
        if (layer != null)
            _mapRenderer.Draw(layer: layer, viewMatrix: Camera2D.GetViewMatrix() * ScaleMatrix);
        if (debug)
        {
            DrawCollisions(_spriteBatch);
        }
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        // Dessine ta map
        _mapRenderer.Draw(viewMatrix: Camera2D.GetViewMatrix() * ScaleMatrix);

        if (debug)
        {
            DrawCollisions(_spriteBatch);
            Shape.DrawRectangle(_spriteBatch, Rect, Color.Blue);
        }
    }
}