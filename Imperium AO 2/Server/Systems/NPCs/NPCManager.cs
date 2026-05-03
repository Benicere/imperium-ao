using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Systems.NPCs;

public class NPCManager : INPCManager
{
    private readonly Dictionary<int, NPC> _npcs = new();
    private readonly Dictionary<int, List<int>> _npcsByMap = new();
    private readonly ILogger<NPCManager> _logger;

    public NPCManager(ILogger<NPCManager> logger)
    {
        _logger = logger;
    }

    public void AddNPC(NPC npc)
    {
        if (_npcs.ContainsKey(npc.Id))
        {
            _logger.LogWarning($"NPC {npc.Id} already exists");
            return;
        }

        _npcs[npc.Id] = npc;

        if (!_npcsByMap.ContainsKey(npc.MapId))
        {
            _npcsByMap[npc.MapId] = new();
        }

        _npcsByMap[npc.MapId].Add(npc.Id);
        _logger.LogInformation($"NPC {npc.Name} (ID: {npc.Id}) added to map {npc.MapId}");
    }

    public void RemoveNPC(int npcId)
    {
        if (!_npcs.TryGetValue(npcId, out var npc))
        {
            _logger.LogWarning($"NPC {npcId} not found");
            return;
        }

        _npcs.Remove(npcId);
        if (_npcsByMap.TryGetValue(npc.MapId, out var mapNpcs))
        {
            mapNpcs.Remove(npcId);
        }

        _logger.LogInformation($"NPC {npc.Name} (ID: {npcId}) removed");
    }

    public NPC? GetNPC(int npcId)
    {
        _npcs.TryGetValue(npcId, out var npc);
        return npc;
    }

    public List<NPC> GetNPCsByMap(int mapId)
    {
        if (!_npcsByMap.TryGetValue(mapId, out var npcIds))
        {
            return new();
        }

        return npcIds
            .Select(id => _npcs[id])
            .ToList();
    }

    public List<NPC> GetNearbyNPCs(int mapId, int x, int y, int range)
    {
        var mapNpcs = GetNPCsByMap(mapId);
        return mapNpcs
            .Where(npc => {
                var distance = Math.Sqrt(Math.Pow(npc.X - x, 2) + Math.Pow(npc.Y - y, 2));
                return distance <= range;
            })
            .ToList();
    }

    public void UpdateNPC(NPC npc)
    {
        if (_npcs.ContainsKey(npc.Id))
        {
            _npcs[npc.Id] = npc;
        }
    }
}


