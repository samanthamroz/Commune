using System.Collections.Generic;
using UnityEngine;

namespace ClickIt.Backend {
    internal class Mouse : IMouse {
        public Vector2 ScreenPosition { get; private set; }
        private Dictionary<MouseButton, bool> _buttonsDown;
        internal Mouse() {
            ScreenPosition = new(0, 0);
            _buttonsDown = new() { [MouseButton.left] = false, [MouseButton.right] = false, [MouseButton.middle] = false };
        }

        public void SetScreenPosition(Vector2 newScreenPosition) {
            bool isValidCoords = newScreenPosition.x < Screen.width &&
                                newScreenPosition.x > 0 &&
                                newScreenPosition.y < Screen.height &&
                                newScreenPosition.y > 0;

            if (isValidCoords) {
                ScreenPosition = newScreenPosition;
            }
        }

        public void SetButtonDown(MouseButton button) {
            _buttonsDown[button] = true;
        }

        public void SetButtonUp(MouseButton button) {
            _buttonsDown[button] = false;
        }

        public bool IsButtonDown(MouseButton button) {
            return _buttonsDown[button];
        }
    }
}