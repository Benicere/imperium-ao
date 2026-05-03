using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.NPCs;

public interface INPCManager
{
    void AddNPC(NPC npc);
    void RemoveNPC(int npcId);
    NPC? GetNPC(int npcId);
    List<NPC> GetNPCsByMap(int mapId);
    List<NPC> GetNearbyNPCs(int mapId, int x, int y, int range);
    void UpdateNPC(NPC npc);
}

