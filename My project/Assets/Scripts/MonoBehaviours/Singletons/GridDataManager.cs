using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

/*
This class holds the data on the grid
*/
public class GridDataManager : MonoBehaviour {
    public static GridDataManager Self;
    [SerializeField] bool createGridFromFile;
    [SerializeField] TileData[] allTileData;
    Dictionary<int, TileData> tileDataRegistry = new();

    [SerializeField] GridDisplay gridDisplay;
    PathCharter pathCharter;

    [SerializeField] int columns = 16, rows = 8;
    DirectionalGridCell[,] gridDirectionalCells;
    Tile[,] gridTiles;

    private void Awake() {
        if (Self == null) {
            Self = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (!createGridFromFile) {
            gridDirectionalCells = CreateBlankGrid();
        } else {
            gridDirectionalCells = GridSerializer.Load(Path.Combine(Application.streamingAssetsPath, "grid.json"));
        }

        foreach (var data in allTileData) tileDataRegistry[data.id] = data;

        gridTiles = CreateTileGrid();

        pathCharter = new();
    }

    public DirectionalGridCell[,] CreateBlankGrid() {
        DirectionalGridCell[,] grid = new DirectionalGridCell[columns, rows];

        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                grid[i, j] = new(0, 0);
            }
        }

        return grid;
    }
    public Tile[,] CreateTileGrid() {
        Tile[,] tiles = new Tile[gridDirectionalCells.GetLength(0), gridDirectionalCells.GetLength(1)];

        for (int i = 0; i < tiles.GetLength(0); i++) {
            for (int j = 0; j < tiles.GetLength(1); j++) {
                DirectionalGridCell cell = gridDirectionalCells[i, j];
                TileData data = GetTileDataForId(cell.cellValue);
                Vector2Int pos = new(i, j);

                var inst = Instantiate(data.prefab, gridDisplay.transform);
                Tile tile = TileFactory.Create(data, pos, inst);
                gridDisplay.PlaceObjectOnGrid(tile.Instance, pos);

                if (tile.Type == TileType.Road) {
                    tile.Instance.GetComponent<RoadSpriteSetter>().ChangeSprite(cell.directionsOpen);
                }

                tiles[i, j] = tile;
            }
        }

        return tiles;
    }
    public void AddToGrid(DragToBuildSquare obj) {
        Tile newTile = TryCreateNewTile(obj);
        if (newTile == null) return;

        Vector2Int cellPos = new(newTile.GridPosition.x, newTile.GridPosition.y);

        gridDirectionalCells[cellPos.x, cellPos.y] = new(ConvertTileTypeToId(newTile.Type), obj.directionOpen);
        gridTiles[cellPos.x, cellPos.y] = newTile;

        ChangeTileSpriteIfRoad(cellPos, newTile.Instance, newTile.Type == TileType.Road);

        ChangeAdjacentRoadSprites(cellPos);
    }
    public Tile TryCreateNewTile(DragToBuildSquare obj) {
        Vector2Int cellPos = gridDisplay.GetGridPosition(obj.gameObject);

        if (!IsWithinGridBounds(cellPos)) return null;

        if (obj.tileData.needsNeighboringRoad && !IsConnectedToRoad(obj.directionOpen, cellPos)) {
            //Debug.Log("Not connected to road!");
            return null;
        }

        if (obj.tileData.type != TileType.Empty && gridTiles[cellPos.x, cellPos.y].Type != TileType.Empty) {
            //Debug.Log("Not empty!");
            return null;
        }

        Destroy(gridTiles[cellPos.x, cellPos.y].Instance);

        var inst = Instantiate(obj.tileData.prefab, transform);
        Tile tile = TileFactory.Create(obj.tileData, cellPos, inst);
        tile.Instance.transform.rotation = obj.transform.rotation;
        gridDisplay.PlaceObjectOnGrid(tile.Instance, cellPos);

        return tile;
    }
    
    //Fix these 3
    private bool IsConnectedToRoad(Direction objDirectionsOpen, Vector2Int cellPos) {
        foreach (Vector2Int v in DirectionMapper.GetVectors(objDirectionsOpen)) {
            Vector2Int neighboringPos = cellPos + v;
            var neighborDirections = GridDataManager.Self.TryGetDirectionsOpen(neighboringPos);

            if (neighborDirections == null) break;

            var oppositionDirection = DirectionMapper.GetDirection(v * -1);

            if (!((Direction)neighborDirections).HasFlag(oppositionDirection)) return false;

            var neighborType = GridDataManager.Self.TryGetTileType(neighboringPos);
            if (neighborType == null) break;
            if (neighborType != TileType.Road) return false;
        }

        return true;
    }
    private void ChangeTileSpriteIfRoad(Vector2Int cellPos, GameObject tileInstance, bool isRoad) {
        if (!isRoad) return;

        List<Vector2Int> vectors = new();

        foreach (Vector2Int v in DirectionMapper.GetAllDirections()) {
            Vector2Int neighboringPos = cellPos + v;
            var neighboringDirections = TryGetDirectionsOpen(neighboringPos);

            if (neighboringDirections == null) break;

            var oppositionDirection = DirectionMapper.GetDirection(v * -1);
            
            if (IsWithinGridBounds(neighboringPos)) {
                if (((Direction)neighboringDirections).HasFlag(oppositionDirection)) vectors.Add(v);
            }
        }
        
        Direction? d = DirectionMapper.GetDirection(vectors);

        if (d == null) {
            d = Direction.None;
        }

        tileInstance.GetComponent<RoadSpriteSetter>().ChangeSprite((Direction)d);
    }
    private void ChangeAdjacentRoadSprites(Vector2Int cellPos) {
        foreach (Vector2Int v in DirectionMapper.GetAllDirections()) {
            Vector2Int adjPos = cellPos + v;

            var tileInstance = TryGetTileInstance(adjPos);
            if (tileInstance == null) break;

            var tileType = TryGetTileType(adjPos);
            if (tileType == null) break;

            ChangeTileSpriteIfRoad(adjPos, tileInstance, tileType == TileType.Road);
        }
    }

    public TileData GetTileDataForId(int id) {
        return tileDataRegistry[id];
    }
    public int ConvertTileTypeToId(TileType tileType) {
        foreach (var kvp in tileDataRegistry) {
            if (kvp.Value.type == tileType) return kvp.Key;
        }
        return -1;
    }

    public int GetNumRowsInGrid() {
        return gridDirectionalCells.GetLength(0);
    }
    public int GetNumColsInGrid() {
        return gridDirectionalCells.GetLength(1);
    }
    public bool IsWithinGridBounds(Vector2Int point) {
        return point.x >= 0 && point.y >= 0 && point.x < GetNumRowsInGrid() && point.y < GetNumColsInGrid();
    }
    
    public Direction? TryGetDirectionsOpen(Vector2Int point) {
        if (!IsWithinGridBounds(point)) return null;

        return gridDirectionalCells[point.x, point.y].directionsOpen;
    }
    public NeedType? TryGetTileNeed(Vector2Int point) {
        if (!IsWithinGridBounds(point)) return null;

        return gridTiles[point.x, point.y].Need;
    }
    public TileType? TryGetTileType(Vector2Int point) {
        if (!IsWithinGridBounds(point)) return null;

        return gridTiles[point.x, point.y].Type;
    }
    public GameObject TryGetTileInstance(Vector2Int point) {
        if (!IsWithinGridBounds(point)) return null;

        return gridTiles[point.x, point.y].Instance;
    }

    public Vector2Int? TryGetRandomPointOfType(TileType tileType) {
        return pathCharter.TryGetRandomPointOfType(gridDirectionalCells, ConvertTileTypeToId(tileType));
    }
    public Vector2Int? TryGetClosestPointOfType(Vector2Int start, TileType tileType) {
        return pathCharter.TryGetClosestPointOfType(gridDirectionalCells, start, ConvertTileTypeToId(tileType));
    }
    public List<Vector2Int> GetAllPointsOfType(TileType tileType) {
        List<Vector2Int> points = new();

        for (int i = 0; i < gridTiles.GetLength(0); i++) {
            for (int j = 0; j < gridTiles.GetLength(1); j++) {
                if (gridTiles[i,j].Type == tileType) points.Add(new(i, j));
            }
        }

        return points;
    }
    
    public List<Vector2Int> TryGetPath(Vector2Int start, Vector2Int end) {
        return pathCharter.TryChartPath(gridDirectionalCells, start, end);
    }
}