using ClickIt;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildingSpawnerButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TileData newTile;
    [SerializeField] GameObject prefabToSpawn;

    public void OnPointerDown(PointerEventData eventData)
    {
        SpawnPrefab();
    }

    void SpawnPrefab() {
        var obj = Instantiate(prefabToSpawn, Camera.main.ScreenToWorldPoint(ClickItCore.Instance.GetMousePosition()), Quaternion.identity);
        obj.GetComponent<DragToBuildSquare>().tileData = newTile;
    }
}
