using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridDisplay : MonoBehaviour {
    [SerializeField] GameObject residentPrefab;
    private Grid grid;
    private GridLayout gridLayout;

    void Awake() {
        grid = GetComponent<Grid>();
        gridLayout = grid.GetComponent<GridLayout>();
    }
    
    public void PlaceObjectOnGrid(GameObject obj, Vector2Int gridPos) {
        obj.transform.position = gridLayout.CellToWorld((Vector3Int)gridPos);
    }
    public Vector2Int GetGridPosition(GameObject obj) {
        return (Vector2Int)gridLayout.WorldToCell(obj.transform.position + new Vector3(.5f, .5f, 0f));
    }
    
    public GameObject SpawnResident(Vector2Int pos) {
        var temp = Instantiate(residentPrefab, gridLayout.CellToWorld((Vector3Int)pos), Quaternion.identity, transform);
        temp.GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV(0f, 1f, .7f, .8f, 0.75f, .9f);
        return temp;
    }

    public void MoveResident(Resident resident, Vector2Int newPos) {
        resident.Instance.transform.position = gridLayout.CellToWorld((Vector3Int)newPos);
    }
}