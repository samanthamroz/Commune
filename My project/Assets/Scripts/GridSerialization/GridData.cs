using System;

// Flat, serializable wrapper around a 2D grid of DirectionalGridCell.
// JsonUtility cannot serialize a T[,] or T[][] directly, so this class
// stores the grid as a single 1D array plus the width/height needed
// to reconstruct the 2D layout.
[Serializable]
public class GridData
{
    public int width;
    public int height;
    public DirectionalGridCell[] grid;

    // Convenience indexer so you can read/write using (x, y)
    // instead of doing the y * width + x math everywhere.
    public DirectionalGridCell this[int x, int y]
    {
        get => grid[y * width + x];
        set => grid[y * width + x] = value;
    }
}