using System.Collections.Generic;
using UnityEngine;

public class PathCharter {
    private class AStarNode {
        public Vector2Int point;
        public float g, h, f;
        public AStarNode parent;
        public AStarNode(Vector2Int point, float g, float h, AStarNode parent) {
            this.point = point;
            this.g = g;
            this.h = h;
            this.f = g + h;
            this.parent = parent;
        }
    }

    public List<Vector2Int> TryChartPath(DirectionalGridCell[,] gridData, Vector2Int startingPoint, Vector2Int endingPoint) {
        float cost = 1f;

        float tempH = GetHeuristic(startingPoint, endingPoint);
        List<AStarNode> open = new() { new(startingPoint, 0, tempH, null) };
        List<AStarNode> closed = new();

        List<AStarNode> neighbors;
        AStarNode n;

        while (open.Count > 0) {
            n = open[0];
            foreach (AStarNode node in open) {
                //investigate the most likely node
                if (node.f < n.f) {
                    n = node;
                }
            }

            if (n.point == endingPoint) return ConstructPath(n);

            //Debug.Log("Open count: " + open.Count);
            //Debug.Log("Investigating: " + n.point + " (" + gridData[n.point.x,n.point.y] + ")");
            
            open.Remove(n);
            closed.Add(n);

            //only look at neighbors the current square is open to
            neighbors = new();
            Vector2Int neighborPoint;
            foreach(Vector2Int v in DirectionMapper.GetVectors(gridData[n.point.x, n.point.y].directionsOpen)) {
                neighborPoint = n.point + v;
                neighbors.Add(new(neighborPoint, n.g + cost, GetHeuristic(neighborPoint, endingPoint), n));
            }

            //only add nodes to the open list if they aren't empty, haven't already been closed, and aren't already on the open list
            foreach (AStarNode node in neighbors) {
                if (ContainsNodeWithPoint(closed, node.point) /*|| gridData[node.point.x, node.point.y].cellValue == 0*/) continue;
                if (!open.Contains(node)) {
                    open.Add(node);
                }
            }
        }

        return null;
    }

    private float GetHeuristic(Vector2Int point, Vector2Int endingPoint) {
        return Vector2.Distance(point, endingPoint);
    }

    private List<Vector2Int> ConstructPath(AStarNode endingNode) {
        List<Vector2Int> path = new();
        AStarNode current = endingNode;
        while (current != null) {
            path.Add(current.point);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private bool ContainsNodeWithPoint(List<AStarNode> list, Vector2Int point) {
        foreach (AStarNode node in list) {
            if (node.point == point) return true;
        }
        return false;
    }

    private List<Vector2Int> GetAllPointsOfType(DirectionalGridCell[,] gridData, int type) {
        List<Vector2Int> points = new();

        for (int i = 0; i < gridData.GetLength(0); i++) {
            for (int j = 0; j < gridData.GetLength(1); j++) {
                if (gridData[i,j].cellValue == type) points.Add(new(i, j));
            }
        }

        return points;
    }

    public Vector2Int? TryGetClosestPointOfType(DirectionalGridCell[,] gridData, Vector2Int start, int type) {
        Vector2Int? chosenPoint = null;

        float tempH = Mathf.Infinity;
        float bestH = tempH;
        foreach (Vector2Int point in GetAllPointsOfType(gridData, type)) {
            tempH = GetHeuristic(start, point);
            if (tempH < bestH) {
                bestH = tempH;
                chosenPoint = point;
            }
        }

        return chosenPoint;
    }

    public Vector2Int? TryGetRandomPointOfType(DirectionalGridCell[,] gridData, int type) {
        Vector2Int? chosenPoint = null;

        var list = GetAllPointsOfType(gridData, type);
        if (list.Count > 0) chosenPoint = list[Random.Range(0, list.Count)];

        return chosenPoint;
    }

    public void PrintPath(Vector2Int[] path) {
        string p = "Path: ";
        foreach (Vector2 v in path) {
            p += v.ToString() + " ";
        }
        Debug.Log(p);
    }

    public void PrintPath(DirectionalGridCell[,] gridData, Vector2Int startingPoint, Vector2Int endingPoint) {
        string p = "Path: ";
        foreach (Vector2 v in TryChartPath(gridData, startingPoint, endingPoint)) {
            p += v.ToString() + " ";
        }
        Debug.Log(p);
    }
}
