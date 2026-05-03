using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Systems.World;

public class WorldSyncSystem : IWorldSyncSystem
{
    private readonly Dictionary<int, WorldObject> _objects = new();
    private readonly Dictionary<int, WorldRegion> _regions = new();
    private readonly Dictionary<int, int> _playerRegions = new();
    private readonly ILogger<WorldSyncSystem> _logger;
    private int _objectIdCounter = 1;

    public WorldSyncSystem(ILogger<WorldSyncSystem> logger)
    {
        _logger = logger;
        InitializeRegions();
    }

    private void InitializeRegions()
    {
        var regionSize = 100;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                var regionId = x * 5 + y;
                _regions[regionId] = new()
                {
                    RegionId = regionId,
                    MinX = x * regionSize,
                    MaxX = (x + 1) * regionSize - 1,
                    MinY = y * regionSize,
                    MaxY = (y + 1) * regionSize - 1,
                    RegionName = $"Region_{x}_{y}"
                };
            }
        }
    }

    public void RegisterObject(WorldObject obj)
    {
        obj.Id = _objectIdCounter++;
        obj.CreatedAt = DateTime.UtcNow;
        _objects[obj.Id] = obj;

        var region = GetRegionContaining(obj.X, obj.Y);
        if (region != null)
        {
            region.AddObject(obj.Id);
            _logger.LogInformation($"Object {obj.Id} ({obj.Name}) registered in region {region.RegionId}");
        }
    }

    public void UnregisterObject(int objectId)
    {
        if (_objects.TryGetValue(objectId, out var obj))
        {
            var region = GetRegionContaining(obj.X, obj.Y);
            if (region != null)
            {
                region.RemoveObject(objectId);
            }

            _objects.Remove(objectId);
            _logger.LogInformation($"Object {objectId} unregistered");
        }
    }

    public void UpdateObjectPosition(int objectId, int x, int y, int z)
    {
        if (_objects.TryGetValue(objectId, out var obj))
        {
            var oldRegion = GetRegionContaining(obj.X, obj.Y);
            var newRegion = GetRegionContaining(x, y);

            obj.X = x;
            obj.Y = y;
            obj.Z = z;

            if (oldRegion?.RegionId != newRegion?.RegionId)
            {
                oldRegion?.RemoveObject(objectId);
                newRegion?.AddObject(objectId);
                _logger.LogInformation($"Object {objectId} moved to region {newRegion?.RegionId}");
            }
        }
    }

    public WorldObject? GetObject(int objectId)
    {
        _objects.TryGetValue(objectId, out var obj);
        return obj;
    }

    public List<WorldObject> GetObjectsInRegion(int regionId)
    {
        if (_regions.TryGetValue(regionId, out var region))
        {
            return region.ObjectIds.Select(id => _objects[id]).Where(o => o != null).ToList();
        }
        return new();
    }

    public List<WorldObject> GetVisibleObjects(int playerId)
    {
        if (!_playerRegions.TryGetValue(playerId, out var regionId))
        {
            return new();
        }

        return GetObjectsInRegion(regionId).Where(o => o.IsVisible).ToList();
    }

    public void SyncRegion(int regionId)
    {
        if (_regions.TryGetValue(regionId, out var region))
        {
            region.LastSyncAt = DateTime.UtcNow;
            _logger.LogInformation($"Region {regionId} synchronized");
        }
    }

    public void AddPlayerToRegion(int playerId, int regionId)
    {
        _playerRegions[playerId] = regionId;
        if (_regions.TryGetValue(regionId, out var region))
        {
            region.PlayerIds.Add(playerId);
            _logger.LogInformation($"Player {playerId} added to region {regionId}");
        }
    }

    public void RemovePlayerFromRegion(int playerId, int regionId)
    {
        _playerRegions.Remove(playerId);
        if (_regions.TryGetValue(regionId, out var region))
        {
            region.PlayerIds.Remove(playerId);
            _logger.LogInformation($"Player {playerId} removed from region {regionId}");
        }
    }

    public WorldRegion? GetRegion(int regionId)
    {
        _regions.TryGetValue(regionId, out var region);
        return region;
    }

    public WorldRegion? GetRegionContaining(int x, int y)
    {
        return _regions.Values.FirstOrDefault(r => r.Contains(x, y));
    }

    public void BroadcastUpdate(int regionId, byte[] data)
    {
        if (_regions.TryGetValue(regionId, out var region))
        {
            _logger.LogInformation($"Broadcast update sent to region {regionId} ({region.PlayerIds.Count} players)");
        }
    }
}


