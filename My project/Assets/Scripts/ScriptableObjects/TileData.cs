using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/TileData")]
public class TileData : ScriptableObject
{
    //shared data fields for all instances of this type
    public int id;
    public TileType type;
    [SerializeReference] public List<TileModData> tileModDatas;
    public NeedType needsSatisfiable;
    public GameObject prefab;
    public bool needsNeighboringRoad;
}