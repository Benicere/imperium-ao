using System;
namespace ImperiumAO.Server.Map;

public class MapData
{
    public int MapId { get; set; }
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 100;
    public string Name { get; set; } = "Unnamed Map";

    // Simple 2D array for blocked tiles (can be expanded)
    public bool[,] BlockedTiles { get; set; }

    public MapData()
    {
        BlockedTiles = new bool[Width, Height];
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;
        return !BlockedTiles[x, y];
    }

    public void SetBlocked(int x, int y, bool blocked)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            BlockedTiles[x, y] = blocked;
    }
}

