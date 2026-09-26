using System.Collections.Generic;
using UnityEngine;

public class Tile //Instance
{
    public readonly TileType Type;
    public readonly NeedType Need;
    public Vector2Int GridPosition;
    public GameObject Instance;
    public List<TileMod> TileMods;

    public Tile(TileData data, Vector2Int pos, GameObject instance) { 
        this.Type = data.type;
        this.Need = data.needsSatisfiable;
        this.GridPosition = pos;
        this.Instance = instance;
        //this.TileMods = mods;
    }
}

