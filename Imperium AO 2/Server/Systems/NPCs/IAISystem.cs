using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.NPCs;

public interface IAISystem
{
    void UpdateAI(float deltaTime);
    void ProcessNPCBehavior(NPC npc);
    bool ShouldAggro(NPC npc, int targetId, int targetX, int targetY);
    void MakeDecision(NPC npc);
    List<int> GetVisibleTargets(NPC npc, List<int> nearbyCharacters);
}

