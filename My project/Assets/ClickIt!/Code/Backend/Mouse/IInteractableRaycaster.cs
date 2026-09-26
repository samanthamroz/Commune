using UnityEngine;

namespace ClickIt.Backend {
    internal interface IInteractableRaycaster {
        public IValidatedObject[] GetInteractableComponentsAtPosition(Vector2 mouseScreenPosition);
        public GameObject GetInteractableGameObjectAtPosition(Vector2 mouseScreenPosition);
    }
}