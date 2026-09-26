using UnityEngine;

namespace ClickIt.Backend {
    internal interface IMouseButtonRouter {
        public void DoLeftClick(Vector2 screenPositionClicked);
        public void DoRightClick(Vector2 screenPositionClicked);
        public void DoMiddleClick(Vector2 screenPositionClicked);
        public void DoLeftRelease();
        public void DoRightRelease();
        public void DoMiddleRelease();
    }
}