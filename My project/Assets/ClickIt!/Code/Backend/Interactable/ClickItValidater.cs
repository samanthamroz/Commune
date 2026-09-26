using UnityEngine;
namespace ClickIt.Backend {
    internal class ClickItValidater : IValidater {
        public void ValidateScene() {
            if (Camera.main == null) {
                Debug.LogError("[ClickIt!] No main camera found. Ensure your scene has a camera tagged 'MainCamera'.");
            }

            if (!AllInteractablesValid()) {
                Debug.LogWarning("[ClickIt!] Some Interactables are not configured properly");
            }
        }
        private bool AllInteractablesValid() {
            foreach (MonoBehaviour mb in GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)) {
                if (mb is IValidatedObject interactable) {
                    if (!interactable.ValidInteractableConfiguration()) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}