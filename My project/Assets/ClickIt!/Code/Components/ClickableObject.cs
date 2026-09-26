using System;
using System.Collections.Generic;
using System.Linq;
using ClickIt.Backend;
using UnityEngine;

namespace ClickIt {
    [AddComponentMenu("ClickIt/ClickableObject")]
    public class ClickableObject : ValidatedObject, IInteractableObject, ILeftClick, IRightClick, IMiddleClick {
        [SerializeField] private List<ClickEventData> clickEvents = new();
        private List<ClickEventData> unserializedClickEvents = new();
        private readonly InteractionEventHandler<ClickEventData> eventHandler = new();

        public void DoLeftClick() {
            eventHandler.ProcessEventsForButton(clickEvents, MouseButton.left);
            eventHandler.ProcessEventsForButton(unserializedClickEvents, MouseButton.left);
        }

        public void DoMiddleClick() {
            eventHandler.ProcessEventsForButton(clickEvents, MouseButton.middle);
            eventHandler.ProcessEventsForButton(unserializedClickEvents, MouseButton.middle);
        }

        public void DoRightClick() {
            eventHandler.ProcessEventsForButton(clickEvents, MouseButton.right);
            eventHandler.ProcessEventsForButton(unserializedClickEvents, MouseButton.right);
        }


        public void AddCallback(MouseButton button, Action callback) {
            AddEvent(callback).Button(button);
        }

        public void RemoveCallback(MouseButton button, Action callback) {
            var eventsToCheck = unserializedClickEvents.Concat(clickEvents);
            
            foreach (var evt in eventsToCheck.Where(e => e.HasButton(button)).ToList()) {
                if (evt.RemoveCallback(callback)) {
                    // Remove event if it has no more callbacks
                    if (evt.CallbackCount == 0) {
                        unserializedClickEvents.Remove(evt as ClickEventData);
                        clickEvents.Remove(evt as ClickEventData);
                    }
                    return;
                }
            }
        }

        public bool HasCallback(MouseButton button, Action callback) {
            return unserializedClickEvents.Concat(clickEvents)
                .Any(e => e.HasButton(button) && e.HasCallback(callback));
        }

        public void ClearCallbacks(MouseButton button) {
            var eventsToRemove = unserializedClickEvents.Concat(clickEvents)
                .Where(e => e.HasButton(button))
                .ToList();
            
            foreach (var evt in eventsToRemove) {
                evt.ClearCallbacks();
                unserializedClickEvents.Remove(evt as ClickEventData);
                clickEvents.Remove(evt as ClickEventData);
            }
        }

        public void ClearAllCallbacks() {
            clickEvents.Clear();
            unserializedClickEvents.Clear();
        }


        public InteractionEventBuilder AddEvent(Action callback) {
            ClickEventData newEvent = new ClickEventData();
            newEvent.AddCallback(callback);

            return new InteractionEventBuilder(newEvent, () => {
                newEvent.ResetTriggerTime();
                unserializedClickEvents.Add(newEvent);
            });
        }

        public void AddEvent(InteractionEventData eventData) {
            unserializedClickEvents.Add((ClickEventData)eventData);
        }

        public void RemoveEvent(InteractionEventData eventData) {
            unserializedClickEvents.Remove(eventData as ClickEventData);
            clickEvents.Remove(eventData as ClickEventData);
        }

        public bool TryRemoveEvent(InteractionEventData eventData) {
            bool removed = unserializedClickEvents.Remove(eventData as ClickEventData);
            removed |= clickEvents.Remove(eventData as ClickEventData);
            return removed;
        }

        public void ClearEvents() {
            clickEvents.Clear();
            unserializedClickEvents.Clear();
        }

        public List<InteractionEventData> GetAllEvents() {
            return clickEvents.Concat(unserializedClickEvents).Cast<InteractionEventData>().ToList();
        }

        public InteractionEventData FindEventByLabel(string label) {
            return clickEvents.Concat(unserializedClickEvents)
                .FirstOrDefault(e => e.Label == label);
        }

        public List<InteractionEventData> FindEventsByLabel(string label) {
            return clickEvents.Concat(unserializedClickEvents)
                .Where(e => e.Label == label)
                .Cast<InteractionEventData>()
                .ToList();
        }

        public List<InteractionEventData> FindEventsByButton(MouseButton button) {
            return clickEvents.Concat(unserializedClickEvents)
                .Where(e => e.HasButton(button))
                .Cast<InteractionEventData>()
                .ToList();
        }
    }
}