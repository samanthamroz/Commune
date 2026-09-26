using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ResidentData")]
public class ResidentData : ScriptableObject
{
    public float hoursUntilRestCritical;
    public float hoursNeededRest;
    public float hoursUntilFoodCritical;
    public float hoursNeededFood;
    public float hoursUntilSocialCritical;
    public float hoursNeededSocial;
}