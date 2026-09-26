using UnityEngine;
using System;
using System.Collections.Generic;

namespace ClickIt.Backend {
    internal class MouseButtonRouter : IMouseButtonRouter {
        private InteractableRaycaster raycaster;
        private Dictionary<MouseButton, IValidatedObject[]> currentInteractablesBeingClicked;
        private Dictionary<MouseButton, IValidatedObject[]> lastInteractablesClicked;

        internal MouseButtonRouter(InteractableRaycaster raycaster) {
            this.raycaster = raycaster;
            currentInteractablesBeingClicked = new Dictionary<MouseButton, IValidatedObject[]> {
                [MouseButton.left] = null,
                [MouseButton.middle] = null,
                [MouseButton.right] = null
            };
            lastInteractablesClicked = new Dictionary<MouseButton, IValidatedObject[]> {
                [MouseButton.left] = null,
                [MouseButton.middle] = null,
                [MouseButton.right] = null
            };
        }

        private bool TryUpdateCurrentInteractable(MouseButton button, Vector2 mouseScreenPosition) {
            var findInteractables = raycaster.GetInteractableComponentsAtPosition(mouseScreenPosition);
            if (findInteractables == null) {
                return false;
            }
            
            currentInteractablesBeingClicked[button] = findInteractables;
            return true;
        }

        private void HandleClick<TClick, TClickAway>(MouseButton button, Vector2 screenPosition, Action<TClick> clickAction, Action<TClickAway> clickAwayAction) {
            // Handle click-away on last interactable
            if (lastInteractablesClicked[button] != null) {
                foreach (IValidatedObject interactable in lastInteractablesClicked[button]) {
                    if (interactable is TClickAway clickAwayObj) {
                        clickAwayAction(clickAwayObj);
                    }
                }
            }

            // Try to find new interactable
            if (!TryUpdateCurrentInteractable(button, screenPosition)) return;

            // Handle click on current interactable
            if (currentInteractablesBeingClicked[button] != null) {
                foreach (IValidatedObject interactable in currentInteractablesBeingClicked[button]) {
                    if (interactable is TClick clickObj) {
                        clickAction(clickObj);
                    }
                }
            }
        }

        private void HandleRelease<TRelease>(MouseButton button, Action<TRelease> releaseAction) {
            if (currentInteractablesBeingClicked[button] != null) {
                foreach (IValidatedObject interactable in currentInteractablesBeingClicked[button]) {
                    if (interactable is TRelease releaseObj) {
                        releaseAction(releaseObj);
                    }
                }
            }

            lastInteractablesClicked[button] = currentInteractablesBeingClicked[button];
            currentInteractablesBeingClicked[button] = null;
        }


        public void DoLeftClick(Vector2 screenPositionClicked) {
            HandleClick<ILeftClick, ILeftClickAway>(MouseButton.left, screenPositionClicked, obj => obj.DoLeftClick(), obj => obj.DoLeftClickAway());
        }

        public void DoRightClick(Vector2 screenPositionClicked) {
            HandleClick<IRightClick, IRightClickAway>(MouseButton.right, screenPositionClicked, obj => obj.DoRightClick(), obj => obj.DoRightClickAway());
        }

        public void DoMiddleClick(Vector2 screenPositionClicked) {
            HandleClick<IMiddleClick, IMiddleClickAway>(MouseButton.middle, screenPositionClicked, obj => obj.DoMiddleClick(), obj => obj.DoMiddleClickAway());
        }


        public void DoLeftRelease() {
            HandleRelease<ILeftRelease>(MouseButton.left, obj => obj.DoLeftRelease());
        }

        public void DoRightRelease() {
            HandleRelease<IRightRelease>(MouseButton.right, obj => obj.DoRightRelease());
        }

        public void DoMiddleRelease() {
            HandleRelease<IMiddleRelease>(MouseButton.middle, obj => obj.DoMiddleRelease());
        }
    }
}