using System.Collections.Generic;
using UnityEngine;

public class Resident {
    ResidentData residentData;
    public Vector2Int GridPosition, HomePosition, WorkPosition;
    public GameObject Instance;
    Vector2Int[] Schedule;
    public List<Vector2Int> CurrentPath;
    public float RestNeed, FoodNeed, SocialNeed = 0;
    float needCriticalPoint = 75, needMaxPoint = 100;

    public Resident(Vector2Int pos, ResidentData residentData, int hoursInDay) {
        this.GridPosition = pos;
        this.HomePosition = pos;
        this.residentData = residentData;
        this.Schedule = new Vector2Int[hoursInDay];
        this.CurrentPath = new();

        for (int i = 0; i < Schedule.Length; i++) Schedule[i] = new(-1, -1);
    }

    public void PrintSchedule() {
        string s = "Schedule: ";
        for (int i = 0; i < Schedule.Length; i++) {
            s += $"\nHour {i}: {Schedule[i]}";
        }
        Debug.Log(s);
    }

    public Vector2Int? TryGetScheduledPoint(int hour) {
        if (hour < 0 || hour >= Schedule.Length) return null;

        return Schedule[hour];
    }

    public void SetScheduledPoint(int hour, int duration, Vector2Int point, bool overrideScheduledEvents) {
        if (hour < 0 || hour >= Schedule.Length) return;

        for (int i = 0; i < duration; i++) {
            if (overrideScheduledEvents || Schedule[(hour + i) % Schedule.Length] == new Vector2Int(-1, -1)) {
                Schedule[(hour + i) % Schedule.Length] = point;
            }
        }
    }

    public void ClearSchedule() {
        for (int i = 0; i < Schedule.Length; i++) Schedule[i] = new(-1, -1);
        SetScheduledPoint(8, 8, WorkPosition, true);
    }
    
    public void UpdateNeeds(NeedType needDecrementing) {
        //every hour
        var restIncrement = needCriticalPoint / residentData.hoursUntilRestCritical;
        var foodIncrement = needCriticalPoint / residentData.hoursUntilFoodCritical;
        var socialIncrement = needCriticalPoint / residentData.hoursUntilSocialCritical;

        RestNeed = Mathf.Clamp(RestNeed + restIncrement, 0, needMaxPoint);
        FoodNeed = Mathf.Clamp(FoodNeed + foodIncrement, 0, needMaxPoint);
        SocialNeed = Mathf.Clamp(SocialNeed + socialIncrement, 0, needMaxPoint);

        float needDecrement;
        switch (needDecrementing) {
            case NeedType.Rest:
                needDecrement = needCriticalPoint / residentData.hoursNeededRest;
                RestNeed = Mathf.Clamp(RestNeed - needDecrement, 0, needMaxPoint);
                break;
            case NeedType.Food:
                needDecrement = needCriticalPoint / residentData.hoursNeededFood;
                FoodNeed = Mathf.Clamp(FoodNeed - needDecrement, 0, needMaxPoint);
                break;
            case NeedType.Social:
                needDecrement = needCriticalPoint / residentData.hoursNeededSocial;
                SocialNeed = Mathf.Clamp(SocialNeed - needDecrement, 0, needMaxPoint);
                break;
        }
    }

    public void SatisfyNeed(int currentHour) {
        //pick need to satisfy
        NeedType pickedNeed;
        if (RestNeed >= needCriticalPoint) pickedNeed = NeedType.Rest;
        else if (FoodNeed >= needCriticalPoint) pickedNeed = NeedType.Food;
        else if (SocialNeed >= needCriticalPoint) pickedNeed = NeedType.Social;
        else {
            int randomInt = (int)Random.Range(0, RestNeed + FoodNeed + SocialNeed);
            if (randomInt < SocialNeed) pickedNeed = NeedType.Social;
            else if (randomInt < SocialNeed + FoodNeed) pickedNeed = NeedType.Food;
            else pickedNeed = NeedType.Rest;
        }

        if (pickedNeed == NeedType.Rest) {
            SetScheduledPoint(currentHour, (int)residentData.hoursNeededRest, HomePosition, false);
            return;
        }

        if (pickedNeed == NeedType.Food) {
            Vector2Int? pointPicked = GridDataManager.Self.TryGetClosestPointOfType(GridPosition, TileType.Food);
            if (pointPicked == null) {
                Debug.Log("No food squares found!");
                return;
            }
            SetScheduledPoint(currentHour, (int)residentData.hoursNeededFood, (Vector2Int)pointPicked, false);
            return;
        }

        if (pickedNeed == NeedType.Social) {
            Vector2Int? pointPicked = GridDataManager.Self.TryGetRandomPointOfType(TileType.Social);
            if (pointPicked == null) {
                Debug.Log("No social squares found!");
                return;
            }
            SetScheduledPoint(currentHour, (int)residentData.hoursNeededSocial, (Vector2Int)pointPicked, false);
            return;
        }

        Debug.Log("dropped out of satisfy");
    }

    public void FollowSchedule(int currentHour) {
        var path = GridDataManager.Self.TryGetPath(GridPosition, (Vector2Int)TryGetScheduledPoint(currentHour));

        if (path == null) {
            Debug.Log("Resident was unable to find a path to their scheduled location");
        }
        else {
            CurrentPath = path;
        }
    }

    
}