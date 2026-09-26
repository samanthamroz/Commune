using System;

namespace ClickIt {
    public class InteractionEventBuilder {
        private InteractionEventData eventData;
        private Action onComplete;
        private bool isBuilt = false;

        internal InteractionEventBuilder(InteractionEventData eventData, Action onComplete) {
            this.eventData = eventData;
            this.onComplete = onComplete;
        }

        private void EnsureBuilt() {
            if (!isBuilt) {
                onComplete?.Invoke();
                isBuilt = true;
            }
        }

        public InteractionEventBuilder AddButton(MouseButton button) {
            // OR the new button with existing buttons
            eventData.SetButtons(eventData.Buttons | button);
            EnsureBuilt();
            return this;
        }

        public InteractionEventBuilder Button(MouseButton button) {
            eventData.SetButtons(button);
            EnsureBuilt();
            return this;
        }

        public InteractionEventBuilder Delay(float seconds) {
            eventData.SetDelay(seconds);
            EnsureBuilt();
            return this;
        }

        public InteractionEventBuilder Cooldown(float seconds) {
            eventData.SetCooldown(seconds);
            EnsureBuilt();
            return this;
        }

        public InteractionEventBuilder Buffer(float seconds) {
            eventData.SetBuffer(seconds);
            EnsureBuilt();
            return this;
        }

        public InteractionEventBuilder Timeout(float seconds) {
            eventData.SetTimeout(seconds);
            EnsureBuilt();
            return this;
        }

        public InteractionEventBuilder Label(string label) {
            eventData.SetLabel(label);
            EnsureBuilt();
            return this;
        }

        public InteractionEventBuilder Enabled(bool enabled) {
            eventData.SetEnabled(enabled);
            EnsureBuilt();
            return this;
        }

        // Implicit conversion
        public static implicit operator InteractionEventData(InteractionEventBuilder builder) {
            builder.EnsureBuilt();
            return builder.eventData;
        }
    }
}