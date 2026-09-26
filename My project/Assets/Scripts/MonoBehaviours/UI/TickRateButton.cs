using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TickRateButton : MonoBehaviour {
    [SerializeField] int newTickRate;
    Button button;

    void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GameEvents.DoTickRateChanged(newTickRate));
    }
}