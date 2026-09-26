using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour {
    [SerializeField] TMP_Text clock, needs;

    private void Awake() {
        GameEvents.OnTimeChanged += UpdateTime;
        GameEvents.OnNeedsChanged += UpdateNeeds;
    }

    private void OnDestroy()
    {
        GameEvents.OnTimeChanged -= UpdateTime;
    }

    public void UpdateTime(int day, int hour, int minute) {
        clock.text = $"Day {day} - {hour:D2}:{minute:D2}";
    }

    public void UpdateNeeds(float rest, float food, float social) {
        needs.text = $"Avg Rest: {rest:F2}\nAvg Food: {food:F2}\nAvg Social: {social:F2}";
    }
}