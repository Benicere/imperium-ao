using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Persistence;

public class PlayerSaveData
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Health { get; set; }
    public int Mana { get; set; }
    public int Gold { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public DateTime SavedAt { get; set; }
    public Dictionary<string, object> CustomData { get; set; } = new();
}

public class WorldSaveData
{
    public int Timestamp { get; set; }
    public List<PlayerSaveData> Players { get; set; } = new();
    public Dictionary<int, object> DynamicObjects { get; set; } = new();
    public Dictionary<string, object> GlobalState { get; set; } = new();
    public DateTime SavedAt { get; set; }
}

public class SavePoint
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public int PlayerCount { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public bool IsValid { get; set; }
}

