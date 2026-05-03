using System.Collections.Generic;
using System;

namespace ImperiumAO.Server.Systems.World;

public interface IWorldSyncSystem
{
    void RegisterObject(WorldObject obj);
    void UnregisterObject(int objectId);
    void UpdateObjectPosition(int objectId, int x, int y, int z);
    WorldObject? GetObject(int objectId);
    List<WorldObject> GetObjectsInRegion(int regionId);
    List<WorldObject> GetVisibleObjects(int playerId);
    void SyncRegion(int regionId);
    void AddPlayerToRegion(int playerId, int regionId);
    void RemovePlayerFromRegion(int playerId, int regionId);
    WorldRegion? GetRegion(int regionId);
    WorldRegion? GetRegionContaining(int x, int y);
    void BroadcastUpdate(int regionId, byte[] data);
}

