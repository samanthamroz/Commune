using UnityEngine;

namespace ClickIt.Backend {
    internal interface IMouse {
        public Vector2 ScreenPosition { get; }

        public void SetScreenPosition(Vector2 newScreenPosition);

        public void SetButtonDown(MouseButton button);

        public void SetButtonUp(MouseButton button);

        public bool IsButtonDown(MouseButton button);
    }
}