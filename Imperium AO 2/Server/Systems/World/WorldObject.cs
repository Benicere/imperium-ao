using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.World;

public enum WorldObjectType
{
    NPC,
    Player,
    Item,
    Effect,
    Building,
    Trap
}

public class WorldObject
{
    public int Id { get; set; }
    public WorldObjectType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LifeSpanSeconds { get; set; }
    public bool IsPersistent { get; set; }
}

public class WorldRegion
{
    public int RegionId { get; set; }
    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public List<int> ObjectIds { get; set; } = new();
    public List<int> PlayerIds { get; set; } = new();
    public DateTime LastSyncAt { get; set; }

    public bool Contains(int x, int y)
    {
        return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
    }

    public void AddObject(int objectId)
    {
        if (!ObjectIds.Contains(objectId))
        {
            ObjectIds.Add(objectId);
            LastSyncAt = DateTime.UtcNow;
        }
    }

    public void RemoveObject(int objectId)
    {
        ObjectIds.Remove(objectId);
        LastSyncAt = DateTime.UtcNow;
    }
}

