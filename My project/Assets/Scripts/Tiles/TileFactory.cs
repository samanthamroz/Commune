using System;
using UnityEngine;

public static class TileFactory
{
    public static Tile Create(TileData data, Vector2Int pos) => data.type switch
    {
        TileType.House => new House(data, pos),
        _ => new Tile(data, pos)
    };
}