using System;
using UnityEngine;

public static class TileFactory
{
    public static Tile Create(TileData data, Vector2Int pos, GameObject instance) {
        return new Tile(data, pos, instance);
    }
}