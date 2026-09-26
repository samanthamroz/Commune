using System.IO;
using UnityEngine;

public static class GridSerializer
{
    // Converts a 2D grid into the flat, serializable GridData form.
    private static GridData Flatten(DirectionalGridCell[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        var data = new GridData
        {
            width = width,
            height = height,
            grid = new DirectionalGridCell[width * height]
        };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[x, y] = grid[x, y];
            }
        }

        return data;
    }

    // Converts flat GridData back into a 2D grid.
    private static DirectionalGridCell[,] Unflatten(GridData data)
    {
        var grid = new DirectionalGridCell[data.width, data.height];

        for (int y = 0; y < data.height; y++)
        {
            for (int x = 0; x < data.width; x++)
            {
                grid[x, y] = data[x, y];
            }
        }

        return grid;
    }

    // Saves a 2D grid to a JSON file at the given path.
    public static void Save(DirectionalGridCell[,] grid, string path)
    {
        GridData data = Flatten(grid);
        string json = JsonUtility.ToJson(data, true); // true = pretty print, easier to hand-edit
        File.WriteAllText(path, json);
    }

    // Loads a 2D grid from a JSON file at the given path.
    public static DirectionalGridCell[,] Load(string path)
    {
        string json = File.ReadAllText(path);
        GridData data = JsonUtility.FromJson<GridData>(json);
        return Unflatten(data);
    }
}