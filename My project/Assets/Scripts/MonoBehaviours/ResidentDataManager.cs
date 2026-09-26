using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentDataManager : MonoBehaviour {
    [SerializeField] GridDataManager gridDataManager;
    [SerializeField] GridDisplay gridDisplay;
    [SerializeField] ResidentData residentData;
    List<Resident>[,] residentsGrid;
    List<Resident> allResidents = new();
    readonly int maxResidentsPerTile = 3;
    
    public void Awake() {
        GameEvents.OnDayChanged += DayUpdate;
        GameEvents.OnHourChanged += HourUpdate;
        GameEvents.OnMinuteChanged += MinuteUpdate;
    }

    public void Start() {
        residentsGrid = new List<Resident>[gridDataManager.GetNumRowsInGrid(), gridDataManager.GetNumColsInGrid()];
        for (int i = 0; i < residentsGrid.GetLength(0); i++) {
            for (int j = 0; j < residentsGrid.GetLength(1); j++) {
                residentsGrid[i, j] = new();
            }
        }

        SpawnResidents();

        DayUpdate(0);
        HourUpdate(0);
    }

    public void MinuteUpdate(int minute) {
        foreach (Resident r in allResidents) {
            bool isAtDestination = r.GridPosition == r.CurrentPath[^1];
            if (isAtDestination) continue;

            Vector2Int nextPoint = r.CurrentPath[minute + 1];
            bool isRoadAheadBlocked = gridDataManager.TryGetTileType(nextPoint) == TileType.Road && residentsGrid[nextPoint.x, nextPoint.y].Count >= maxResidentsPerTile;
            if (isRoadAheadBlocked) {
                //print("Traffic jam");
                r.CurrentPath.Insert(minute + 1, r.GridPosition);
            }
            else {
                MoveResident(r, r.CurrentPath[minute + 1]);
            }
        }
    }

    public void HourUpdate(int hour) {
        float sumRest = 0, sumFood = 0, sumSocial = 0;

        foreach (Resident r in allResidents) {
            r.UpdateNeeds((NeedType)gridDataManager.TryGetTileNeed(r.GridPosition));
            sumRest += r.RestNeed;
            sumFood += r.FoodNeed;
            sumSocial += r.SocialNeed;

            var scheduledPoint = r.TryGetScheduledPoint(hour);

            if (scheduledPoint == null) continue; //invalid hour given
            if (scheduledPoint == new Vector2Int(-1, -1)) { //unscheduled time - add need to schedule
                r.SatisfyNeed(hour);
            }
            r.FollowSchedule(hour);
        }

        GameEvents.DoNeedsChanged(sumRest / allResidents.Count, sumFood / allResidents.Count, sumSocial / allResidents.Count);
    }

    public void DayUpdate(int day) {
        foreach (Resident r in allResidents) {
            r.ClearSchedule();
        }
    }

    public void MoveResident(Resident r, Vector2Int newPos) {
        residentsGrid[r.GridPosition.x, r.GridPosition.y].Remove(r);

        r.GridPosition = newPos;
        residentsGrid[r.GridPosition.x, r.GridPosition.y].Add(r);

        gridDisplay.MoveResident(r, newPos);
    }
    
    //TODO: Fix
    private void SpawnResidents() {
        /*
        foreach (Vector2Int housePoint in gridDataManager.GetAllPointsOfType(TileType.House)) {
            for (int i = 0; i < houseTile.NumResidents; i++) {
                Resident newResident = new(houseTile.GridPosition, residentData);
                newResident.Instance = gridDisplay.SpawnResident(houseTile.GridPosition);

                residentsGrid[houseTile.GridPosition.x, houseTile.GridPosition.y].Add(newResident);
                allResidents.Add(newResident);
                
                var workPos = gridDataManager.TryGetRandomPointOfType(TileType.Work);
                if (workPos == null) {
                    print("No workplaces found!");
                } else {
                    newResident.WorkPosition = (Vector2Int)workPos;
                    newResident.SetScheduledPoint(1, 8, newResident.WorkPosition, true);
                } 
            }
        }
        */
    }
    
}