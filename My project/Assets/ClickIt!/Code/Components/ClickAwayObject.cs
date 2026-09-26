using System;
using System.Collections.Generic;
using System.Linq;
using ClickIt.Backend;
using UnityEngine;

namespace ClickIt {
    [AddComponentMenu("ClickIt/ClickAwayObject")]
    public class ClickAwayObject : ValidatedObject, IInteractableObject, ILeftClickAway, IRightClickAway, IMiddleClickAway {
        [SerializeField] private List<ClickAwayEventData> clickAwayEvents = new();
        private List<ClickAwayEventData> unserializedClickAwayEvents = new();
        private readonly InteractionEventHandler<ClickAwayEventData> eventHandler = new();

        public void DoLeftClickAway() {
            eventHandler.ProcessEventsForButton(clickAwayEvents, MouseButton.left);
            eventHandler.ProcessEventsForButton(unserializedClickAwayEvents, MouseButton.left);
        }

        public void DoMiddleClickAway() {
            eventHandler.ProcessEventsForButton(clickAwayEvents, MouseButton.middle);
            eventHandler.ProcessEventsForButton(unserializedClickAwayEvents, MouseButton.middle);
        }

        public void DoRightClickAway() {
            eventHandler.ProcessEventsForButton(clickAwayEvents, MouseButton.right);
            eventHandler.ProcessEventsForButton(unserializedClickAwayEvents, MouseButton.right);
        }


        public void AddCallback(MouseButton button, Action callback) {
            AddEvent(callback).Button(button);
        }

        public void RemoveCallback(MouseButton button, Action callback) {
            var eventsToCheck = unserializedClickAwayEvents.Concat(clickAwayEvents);
            
            foreach (var evt in eventsToCheck.Where(e => e.HasButton(button)).ToList()) {
                if (evt.RemoveCallback(callback)) {
                    // Remove event if it has no more callbacks
                    if (evt.CallbackCount == 0) {
                        unserializedClickAwayEvents.Remove(evt as ClickAwayEventData);
                        clickAwayEvents.Remove(evt as ClickAwayEventData);
                    }
                    return;
                }
            }
        }

        public bool HasCallback(MouseButton button, Action callback) {
            return unserializedClickAwayEvents.Concat(clickAwayEvents)
                .Any(e => e.HasButton(button) && e.HasCallback(callback));
        }

        public void ClearCallbacks(MouseButton button) {
            var eventsToRemove = unserializedClickAwayEvents.Concat(clickAwayEvents)
                .Where(e => e.HasButton(button))
                .ToList();
            
            foreach (var evt in eventsToRemove) {
                evt.ClearCallbacks();
                unserializedClickAwayEvents.Remove(evt as ClickAwayEventData);
                clickAwayEvents.Remove(evt as ClickAwayEventData);
            }
        }

        public void ClearAllCallbacks() {
            clickAwayEvents.Clear();
            unserializedClickAwayEvents.Clear();
        }


        public InteractionEventBuilder AddEvent(Action callback) {
            ClickAwayEventData newEvent = new ClickAwayEventData();
            newEvent.AddCallback(callback);

            return new InteractionEventBuilder(newEvent, () => {
                newEvent.ResetTriggerTime();
                unserializedClickAwayEvents.Add(newEvent);
            });
        }

        public void AddEvent(InteractionEventData eventData) {
            unserializedClickAwayEvents.Add((ClickAwayEventData)eventData);
        }

        public void RemoveEvent(InteractionEventData eventData) {
            unserializedClickAwayEvents.Remove(eventData as ClickAwayEventData);
            clickAwayEvents.Remove(eventData as ClickAwayEventData);
        }

        public bool TryRemoveEvent(InteractionEventData eventData) {
            bool removed = unserializedClickAwayEvents.Remove(eventData as ClickAwayEventData);
            removed |= clickAwayEvents.Remove(eventData as ClickAwayEventData);
            return removed;
        }

        public void ClearEvents() {
            clickAwayEvents.Clear();
            unserializedClickAwayEvents.Clear();
        }

        public List<InteractionEventData> GetAllEvents() {
            return clickAwayEvents.Concat(unserializedClickAwayEvents).Cast<InteractionEventData>().ToList();
        }

        public InteractionEventData FindEventByLabel(string label) {
            return clickAwayEvents.Concat(unserializedClickAwayEvents)
                .FirstOrDefault(e => e.Label == label);
        }

        public List<InteractionEventData> FindEventsByLabel(string label) {
            return clickAwayEvents.Concat(unserializedClickAwayEvents)
                .Where(e => e.Label == label)
                .Cast<InteractionEventData>()
                .ToList();
        }

        public List<InteractionEventData> FindEventsByButton(MouseButton button) {
            return clickAwayEvents.Concat(unserializedClickAwayEvents)
                .Where(e => e.HasButton(button))
                .Cast<InteractionEventData>()
                .ToList();
        }
    }
}