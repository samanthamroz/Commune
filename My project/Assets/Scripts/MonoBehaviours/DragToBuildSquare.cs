using System.Collections;
using ClickIt;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DragToBuildSquare : MonoBehaviour
{
    bool isDragging = true;
    public TileData tileData;
    [HideInInspector] public Direction directionOpen;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        GetComponent<SpriteRenderer>().sprite = tileData.prefab.GetComponent<SpriteRenderer>().sprite;
        
        ClickItCore.Instance.SimulateClickAtMousePosition(ClickIt.MouseButton.left);
        StartCoroutine(Drag());
    }

    IEnumerator Drag() {
        while (isDragging) {
            var pos = Camera.main.ScreenToWorldPoint(ClickItCore.Instance.GetMousePosition());
            transform.position = new(pos.x, pos.y, 0);
            yield return null;
        }
    }

    public void RotateCW() {
        transform.Rotate(0,0,-90);
        directionOpen = DirectionMapper.Rotate(directionOpen, true);
    }

    public void Drop() {
        isDragging = false;
        GridDataManager.Self.AddToGrid(this);
        Destroy(gameObject);
    }
}
