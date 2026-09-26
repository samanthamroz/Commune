using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/TileData")]
public class TileData : ScriptableObject
{
    //shared data fields for all instances of this type
    public int id;
    public TileType type;
    public NeedType need;
    public GameObject prefab;
    public bool needsNeighboringRoad, spawnsResidents;
    public List<HappinessAffecter> affecters;
}