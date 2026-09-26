using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickIt {
    [Serializable]
    public class InteractionEventData {
        [SerializeField] private string optionalLabel;
        [SerializeField] private UnityEvent evt;
        [SerializeField] private MouseButton buttons = MouseButton.left;
        [SerializeField] private float cooldown;
        [SerializeField] private float timeout;
        [SerializeField] private float bufferTime;
        [SerializeField] private float delay;
        [SerializeField] private bool enabled = true;
        [HideInInspector][SerializeField] private float lastTriggerTime;

        private List<Action> codeCallbacks = new List<Action>();

        public string Label => optionalLabel;
        public MouseButton Buttons => buttons;
        public float Cooldown => cooldown;
        public float Timeout => timeout;
        public float BufferTime => bufferTime;
        public float Delay => delay;
        public bool Enabled => enabled;
        public float LastTriggerTime => lastTriggerTime;

        public float TimeSinceLastTrigger => Time.time - lastTriggerTime;
        public bool IsInCooldown => enabled && TimeSinceLastTrigger < Cooldown && Cooldown != 0;
        public float TimeLeftInCooldown => Mathf.Max(0f, Cooldown - TimeSinceLastTrigger);
        public bool IsInTimeout => enabled && TimeSinceLastTrigger > Timeout && Timeout != 0;
        public float TimeLeftInTimeout => Mathf.Max(0f, Timeout - TimeSinceLastTrigger);
        public bool IsInBuffer => TimeLeftInCooldown <= BufferTime && IsInCooldown;

        public InteractionEventData() {
            enabled = true;
            buttons = MouseButton.left;
            codeCallbacks = new List<Action>();
        }

        internal InteractionEventData SetLabel(string label) {
            optionalLabel = label;
            return this;
        }

        internal InteractionEventData SetButtons(MouseButton buttons) {
            this.buttons = buttons;
            return this;
        }

        internal InteractionEventData SetCooldown(float seconds) {
            cooldown = Mathf.Max(0f, seconds);
            return this;
        }

        internal InteractionEventData SetTimeout(float seconds) {
            timeout = Mathf.Max(0f, seconds);
            return this;
        }

        internal InteractionEventData SetBuffer(float seconds) {
            bufferTime = Mathf.Max(0f, seconds);
            return this;
        }

        internal InteractionEventData SetDelay(float seconds) {
            delay = Mathf.Max(0f, seconds);
            return this;
        }

        internal InteractionEventData SetEnabled(bool isEnabled) {
            if (!enabled && isEnabled) ResetTriggerTime();
            enabled = isEnabled;
            return this;
        }

        internal void AddCallback(Action callback) {
            if (callback == null) return;
            codeCallbacks.Add(callback);
        }

        internal bool RemoveCallback(Action callback) {
            return codeCallbacks.Remove(callback);
        }

        internal bool HasCallback(Action callback) {
            return codeCallbacks.Contains(callback);
        }

        internal void ClearCallbacks() {
            codeCallbacks.Clear();
        }

        internal int CallbackCount => codeCallbacks.Count;

        public bool HasButton(MouseButton button) {
            return (buttons & button) != 0;
        }

        internal void ResetTriggerTime() {
            lastTriggerTime = Time.time;
        }

        internal void TriggerEvent() {
            ResetTriggerTime();
            evt?.Invoke();
            foreach (var callback in codeCallbacks) {
                callback?.Invoke();
            }
        }
    }
    [Serializable] internal class ClickEventData : InteractionEventData { }
    [Serializable] internal class ReleaseEventData : InteractionEventData { }
    [Serializable] internal class ClickAwayEventData : InteractionEventData { }
}