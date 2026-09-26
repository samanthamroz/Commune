using System.Collections;
using UnityEngine;

public class Ticker : MonoBehaviour {
    bool isSimulationPaused = false;
    static readonly WaitForSeconds tickRate0 = new(.5f), tickRate1 = new(.1f), tickRate2 = new(.05f), debug_tickRate3 = new(.001f);
    int currentTickRate = 1, hour = 0, minute = 0, day = 0;
    readonly int minTickRate = 0, maxTickRate = 4, hoursInDay = 24, minutesInHour = 24;

    private void Awake() {
        GameEvents.OnTickRateChanged += ChangeTickRate;
    }

    private void Start() {
        StartCoroutine(Tick());
    }

    private IEnumerator Tick() {
        while (true) {
            while (!isSimulationPaused) {
                minute += 1;
                GameEvents.DoMinuteChanged(minute);

                if (minute == minutesInHour) {
                    minute = 0;
                    hour += 1;

                    GameEvents.DoHourChanged(hour);
                }
                
                if (hour == hoursInDay) {
                    hour = 0;
                    day += 1;

                    GameEvents.DoDayChanged(day);
                }

                GameEvents.DoTimeChanged(day, hour, minute);

                yield return GetTickSeconds();
            }
            yield return tickRate2;
        }
    }

    private WaitForSeconds GetTickSeconds() {
        return currentTickRate switch {
            2 => tickRate1,
            3 => tickRate2,
            4 => debug_tickRate3,
            _ => tickRate0,
        };
    }

    private void ChangeTickRate(int newTickRate) {
        if (newTickRate < minTickRate || newTickRate > maxTickRate) {
            Debug.Log("Invalid tick rate");
            return;
        }

        if (newTickRate == 0) {
            isSimulationPaused = true;
            return;
        }

        isSimulationPaused = false;
        currentTickRate = newTickRate;
    }
}