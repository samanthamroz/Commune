using UnityEngine;

namespace ClickIt.Backend {
    public abstract class ValidatedObject : MonoBehaviour, IValidatedObject {
        public bool ValidInteractableConfiguration() {
            if (!TryGetComponent<Collider>(out _) && !TryGetComponent<Collider2D>(out _)) {
                Debug.LogWarning($"ClickCore >> {gameObject.name} does not include a Collider.");
                return false;
            }

            return true;
        }
    }
}