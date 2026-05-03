using System.Threading.Tasks;
using System;

namespace ImperiumAO.Server.Map;

public interface IMapManager
{
    Task InitializeAsync();
    MapData? GetMap(int mapId);
    bool IsWalkable(int mapId, int x, int y);
}

