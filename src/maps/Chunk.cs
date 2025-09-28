
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

public class Chunk
{
    public static Size ChunkSize = new Size(64, 64);

    public (int X, int Y) position;
    public List<(TiledMapTile tile, int X, int Y)> Tiles;

    public Chunk(int x, int y)
    {
        position.X = x;
        position.Y = y;

        Tiles = new();
    }

    public override string ToString()
    {
        return $"[CHUNK] pos:({position.X},{position.Y}), Tiles: {Tiles.Count}";
    }
}