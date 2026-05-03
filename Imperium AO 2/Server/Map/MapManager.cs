using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Map;

public class MapManager : IMapManager
{
    private readonly Dictionary<int, MapData> _maps = new();
    private readonly ILogger<MapManager> _logger;

    public MapManager(ILogger<MapManager> logger)
    {
        _logger = logger;
    }

    public Task InitializeAsync()
    {
        try
        {
            // Load or create maps
            // For MVP, create a single default map
            var defaultMap = new MapData
            {
                MapId = 1,
                Name = "Mainland",
                Width = 100,
                Height = 100
            };

            _maps[1] = defaultMap;
            _logger.LogInformation("Map {MapId} initialized: {MapName}", defaultMap.MapId, defaultMap.Name);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing maps");
            throw;
        }
    }

    public MapData? GetMap(int mapId)
    {
        _maps.TryGetValue(mapId, out var map);
        return map;
    }

    public bool IsWalkable(int mapId, int x, int y)
    {
        var map = GetMap(mapId);
        if (map == null)
            return false;
        return map.IsWalkable(x, y);
    }
}
