using UnityEngine;

public class House : Tile
{
    public int NumResidents = 1;
    public House(TileData data, Vector2Int pos) : base(data, pos) { }
}