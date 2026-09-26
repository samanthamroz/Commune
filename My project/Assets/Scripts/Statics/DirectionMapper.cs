using System.Collections.Generic;
using UnityEngine;

public static class DirectionMapper {
    private static Dictionary<Direction, Vector2Int> map = new() {
        {Direction.North, Vector2Int.up},
        {Direction.East, Vector2Int.right},
        {Direction.South, Vector2Int.down},
        {Direction.West, Vector2Int.left}
    };

    private static readonly Dictionary<Vector2Int, Direction> reverseMap = BuildReverseMap();

    private static Dictionary<Vector2Int, Direction> BuildReverseMap() {
        var reverse = new Dictionary<Vector2Int, Direction>();
        foreach (var kvp in map) {
            reverse[kvp.Value] = kvp.Key;
        }
        return reverse;
    }

    public static List<Vector2Int> GetAllDirections() {
        List<Vector2Int> v = new();
        foreach (var value in map.Values) {
            v.Add(value);
        }
        return v;
    }
    
    public static List<Vector2Int> GetVectors(Direction d) {
        List<Vector2Int> v = new();

        foreach (Direction direction in map.Keys) {
            if (d.HasFlag(direction)) v.Add(map[direction]);
        }

        return v;
    }

    public static Direction? GetDirection(Vector2Int vector) {
        if (reverseMap.TryGetValue(vector, out Direction direction)) {
            return direction;
        }
        return null;
    }

    public static Direction? GetDirection(List<Vector2Int> vectors) {
        Direction d = Direction.None;

        foreach (Vector2Int v in vectors) {
            if (reverseMap.TryGetValue(v, out Direction direction)) {
                d |= direction;
            }
        }

        if (d == Direction.None) return null;

        return d;
    }

    public static Direction Combine(Direction d1, Direction d2) {
        return d1 |= d2;
    }

    public static Direction? Combine(Direction d, Vector2Int vector) {
        Direction? d2 = GetDirection(vector);

        if (d2 == null) return null;

        return d |= (Direction)d2;
    }

    public static Direction? Combine(Direction d, List<Vector2Int> vectors) {
        Direction? d2 = GetDirection(vectors);

        if (d2 == null) return null;

        return d |= (Direction)d2;
    }

    public static Direction Rotate(Direction d, bool clockwise) {
        if (clockwise) {
            return (Direction)((int)d * 2 % 15);
        } else {
            return (Direction)((int)d * 8 % 15);
        }
    }
}