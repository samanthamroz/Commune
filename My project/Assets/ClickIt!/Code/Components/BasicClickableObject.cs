using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ClickIt.Backend;

namespace ClickIt {
    [AddComponentMenu("ClickIt/BasicClickableObject")]
    public class BasicClickableObject : ValidatedObject, IInteractableObject, ILeftClick, IRightClick, IMiddleClick {
        [SerializeField] private UnityEvent onLeftClick;
        [SerializeField] private UnityEvent onRightClick;
        [SerializeField] private UnityEvent onMiddleClick;

        private Dictionary<MouseButton, List<Action>> callbacks = new();

        private void Awake() {
            callbacks[MouseButton.left] = new();
            callbacks[MouseButton.right] = new();
            callbacks[MouseButton.middle] = new();
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

        public void ClearAllCallbacks() {
            foreach (var list in callbacks.Values) {
                list.Clear();
            }
        }

        public void DoLeftClick() {
            onLeftClick?.Invoke();
            InvokeCallbacks(MouseButton.left);
        }

        public void DoRightClick() {
            onRightClick?.Invoke();
            InvokeCallbacks(MouseButton.right);
        }

        public void DoMiddleClick() {
            onMiddleClick?.Invoke();
            InvokeCallbacks(MouseButton.middle);
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