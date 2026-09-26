using System;
using System.Collections.Generic;
using System.Linq;
using ClickIt.Backend;
using UnityEngine;

namespace ClickIt {
    [AddComponentMenu("ClickIt/ReleaseableObject")]
    public class ReleaseableObject : ValidatedObject, IInteractableObject, ILeftRelease, IRightRelease, IMiddleRelease {
        [SerializeField] private List<ReleaseEventData> releaseEvents = new();
        private List<ReleaseEventData> unserializedReleaseEvents = new();
        private readonly InteractionEventHandler<ReleaseEventData> eventHandler = new();

        public void DoLeftRelease() {
            eventHandler.ProcessEventsForButton(releaseEvents, MouseButton.left);
            eventHandler.ProcessEventsForButton(unserializedReleaseEvents, MouseButton.left);
        }

        public void DoMiddleRelease() {
            eventHandler.ProcessEventsForButton(releaseEvents, MouseButton.middle);
            eventHandler.ProcessEventsForButton(unserializedReleaseEvents, MouseButton.middle);
        }

        public void DoRightRelease() {
            eventHandler.ProcessEventsForButton(releaseEvents, MouseButton.right);
            eventHandler.ProcessEventsForButton(unserializedReleaseEvents, MouseButton.right);
        }


        public void AddCallback(MouseButton button, Action callback) {
            AddEvent(callback).Button(button);
        }

        public void RemoveCallback(MouseButton button, Action callback) {
            var eventsToCheck = unserializedReleaseEvents.Concat(releaseEvents);
            
            foreach (var evt in eventsToCheck.Where(e => e.HasButton(button)).ToList()) {
                if (evt.RemoveCallback(callback)) {
                    // Remove event if it has no more callbacks
                    if (evt.CallbackCount == 0) {
                        unserializedReleaseEvents.Remove(evt as ReleaseEventData);
                        releaseEvents.Remove(evt as ReleaseEventData);
                    }
                    return;
                }
            }
        }

        public bool HasCallback(MouseButton button, Action callback) {
            return unserializedReleaseEvents.Concat(releaseEvents)
                .Any(e => e.HasButton(button) && e.HasCallback(callback));
        }

        public void ClearCallbacks(MouseButton button) {
            var eventsToRemove = unserializedReleaseEvents.Concat(releaseEvents)
                .Where(e => e.HasButton(button))
                .ToList();
            
            foreach (var evt in eventsToRemove) {
                evt.ClearCallbacks();
                unserializedReleaseEvents.Remove(evt as ReleaseEventData);
                releaseEvents.Remove(evt as ReleaseEventData);
            }
        }

        public void ClearAllCallbacks() {
            releaseEvents.Clear();
            unserializedReleaseEvents.Clear();
        }


        public InteractionEventBuilder AddEvent(Action callback) {
            ReleaseEventData newEvent = new ReleaseEventData();
            newEvent.AddCallback(callback);

            return new InteractionEventBuilder(newEvent, () => {
                newEvent.ResetTriggerTime();
                unserializedReleaseEvents.Add(newEvent);
            });
        }

        public void AddEvent(InteractionEventData eventData) {
            unserializedReleaseEvents.Add((ReleaseEventData)eventData);
        }

        public void RemoveEvent(InteractionEventData eventData) {
            unserializedReleaseEvents.Remove(eventData as ReleaseEventData);
            releaseEvents.Remove(eventData as ReleaseEventData);
        }

        public bool TryRemoveEvent(InteractionEventData eventData) {
            bool removed = unserializedReleaseEvents.Remove(eventData as ReleaseEventData);
            removed |= releaseEvents.Remove(eventData as ReleaseEventData);
            return removed;
        }

        public void ClearEvents() {
            releaseEvents.Clear();
            unserializedReleaseEvents.Clear();
        }

        public List<InteractionEventData> GetAllEvents() {
            return releaseEvents.Concat(unserializedReleaseEvents).Cast<InteractionEventData>().ToList();
        }

        public InteractionEventData FindEventByLabel(string label) {
            return releaseEvents.Concat(unserializedReleaseEvents)
                .FirstOrDefault(e => e.Label == label);
        }

        public List<InteractionEventData> FindEventsByLabel(string label) {
            return releaseEvents.Concat(unserializedReleaseEvents)
                .Where(e => e.Label == label)
                .Cast<InteractionEventData>()
                .ToList();
        }

        public List<InteractionEventData> FindEventsByButton(MouseButton button) {
            return releaseEvents.Concat(unserializedReleaseEvents)
                .Where(e => e.HasButton(button))
                .Cast<InteractionEventData>()
                .ToList();
        }
    }
}