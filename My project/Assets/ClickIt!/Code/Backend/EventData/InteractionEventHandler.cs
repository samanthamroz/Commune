using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClickIt.Backend {
    internal class InteractionEventHandler<TEventData> where TEventData : InteractionEventData {
        internal void ProcessEventsForButton(List<TEventData> events, MouseButton button) {
            var eventsToProcess = events
                    .Where(e => e.Enabled && e.HasButton(button) && !e.IsInTimeout)
                    .ToList();

            foreach (var eventData in eventsToProcess) {
                float timeSinceLastTrigger = Time.time - eventData.LastTriggerTime;

                //event has no cooldown
                if (eventData.Cooldown == 0) {
                    ExecuteEventWithDelayCheck(eventData);
                    continue;
                }

                if (eventData.IsInCooldown) {
                    if (eventData.IsInBuffer) {
                        QueueCooldownTrigger(eventData, timeSinceLastTrigger);
                    }
                    continue;
                }

                ExecuteEventWithDelayCheck(eventData);
            }
        }

        private void ExecuteEventWithDelayCheck(TEventData eventData) {
            if (eventData.Delay > 0) {
                ClickItCore.Instance.DoDelayedAction(eventData.TriggerEvent, eventData.Delay);
            }
            else {
                eventData.TriggerEvent();
            }
        }

        private void QueueCooldownTrigger(TEventData eventData, float timeSinceLastTrigger) {
            float timeInCooldownRemaining = eventData.Cooldown - timeSinceLastTrigger;

            ClickItCore.Instance.DoDelayedAction(() => {
                float newTimeSinceLastTrigger = Time.time - eventData.LastTriggerTime;
                if (newTimeSinceLastTrigger >= eventData.Cooldown) {
                    ExecuteEventWithDelayCheck(eventData);
                }
            }, timeInCooldownRemaining);
        }
    }
}