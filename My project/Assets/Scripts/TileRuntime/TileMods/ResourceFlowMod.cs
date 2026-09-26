using System;
using UnityEngine;

[Serializable]
public class ResourceFlowMod : TileModData
{
    public ResourceType typeFlowing;
    public int ratePerDay;
}