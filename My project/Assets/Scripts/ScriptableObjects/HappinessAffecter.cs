using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HappinessAffecter")]
public class HappinessAffecter : ScriptableObject
{
    public HappinessType typeProvided;
    public int effect;
}