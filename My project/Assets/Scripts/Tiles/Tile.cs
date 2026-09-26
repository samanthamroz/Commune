using UnityEngine;

public class Tile
{
    public TileData Data;
    public Vector2Int GridPosition;
    public GameObject Instance;

    public Tile(TileData data, Vector2Int pos) { 
        this.Data = data; 
        this.GridPosition = pos;
    }
}

