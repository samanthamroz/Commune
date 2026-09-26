using System;

namespace ClickIt {
    internal interface IInteractableObject {
        void AddCallback(MouseButton button, Action callback);
        void RemoveCallback(MouseButton button, Action callback);
        bool HasCallback(MouseButton button, Action callback);
        void ClearCallbacks(MouseButton button);
        void ClearAllCallbacks();
    }
}