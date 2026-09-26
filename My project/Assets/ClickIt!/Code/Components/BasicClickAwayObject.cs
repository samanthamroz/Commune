using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ClickIt.Backend;

namespace ClickIt {
    [AddComponentMenu("ClickIt/BasicClickAwayObject")]
    public class BasicClickAwayObject : ValidatedObject, IInteractableObject, ILeftClickAway, IRightClickAway, IMiddleClickAway {
        [SerializeField] private UnityEvent onLeftClickAway;
        [SerializeField] private UnityEvent onRightClickAway;
        [SerializeField] private UnityEvent onMiddleClickAway;

        private Dictionary<MouseButton, List<Action>> callbacks = new();

        private void Awake() {
            callbacks[MouseButton.left] = new();
            callbacks[MouseButton.right] = new();
            callbacks[MouseButton.middle] = new();
        }

        public void ClearAllCallbacks() {
            foreach (var list in callbacks.Values) {
                list.Clear();
            }
        }

        public void DoLeftClickAway() {
            onLeftClickAway?.Invoke();
            InvokeCallbacks(MouseButton.left);
        }

        public void DoRightClickAway() {
            onRightClickAway?.Invoke();
            InvokeCallbacks(MouseButton.right);
        }

        public void DoMiddleClickAway() {
            onMiddleClickAway?.Invoke();
            InvokeCallbacks(MouseButton.middle);
        }

        public void AddCallback(MouseButton button, Action callback) {
            if (callback == null) return;

            foreach (MouseButton flag in Enum.GetValues(typeof(MouseButton))) {
                if ((button & flag) != 0 && callbacks.ContainsKey(flag)) {
                    callbacks[flag].Add(callback);
                }
            }
        }

        public void RemoveCallback(MouseButton button, Action callback) {
            foreach (MouseButton flag in Enum.GetValues(typeof(MouseButton))) {
                if ((button & flag) != 0 && callbacks.ContainsKey(flag)) {
                    callbacks[flag].Remove(callback);
                }
            }
        }

        public bool HasCallback(MouseButton button, Action callback) {
            foreach (MouseButton flag in Enum.GetValues(typeof(MouseButton))) {
                if ((button & flag) != 0 && callbacks.ContainsKey(flag)) {
                    if (callbacks[flag].Contains(callback)) return true;
                }
            }
            return false;
        }

        public void ClearCallbacks(MouseButton button) {
            foreach (MouseButton flag in Enum.GetValues(typeof(MouseButton))) {
                if ((button & flag) != 0 && callbacks.ContainsKey(flag)) {
                    callbacks[flag].Clear();
                }
            }
        }

        private void InvokeCallbacks(MouseButton button) {
            if (callbacks.TryGetValue(button, out var list)) {
                foreach (var callback in list) {
                    callback?.Invoke();
                }
            }
        }
    }
}