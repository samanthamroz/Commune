using System;

public static class GameEvents {
    public static event Action<int, int, int> OnTimeChanged;
    public static event Action<float, float, float> OnNeedsChanged;
    public static event Action<int> OnTickRateChanged, OnHourChanged, OnMinuteChanged, OnDayChanged;

    public static void DoTimeChanged(int day, int hour, int minute)
    {
        OnTimeChanged?.Invoke(day, hour, minute);
    }

    public static void DoMinuteChanged(int minute) {
        OnMinuteChanged?.Invoke(minute);
    }

    public static void DoHourChanged(int hour) {
        OnHourChanged?.Invoke(hour);
    }

    public static void DoDayChanged(int day) {
        OnHourChanged?.Invoke(day);
    }

    public static void DoNeedsChanged(float rest, float food, float social) {
        OnNeedsChanged?.Invoke(rest, food, social);
    }

    public static void DoTickRateChanged(int newTickRate) {
        OnTickRateChanged?.Invoke(newTickRate);
    }
}