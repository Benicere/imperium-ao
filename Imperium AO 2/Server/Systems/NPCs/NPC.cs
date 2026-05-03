using ImperiumAO.Server.Systems.Combat;
using System;

namespace ImperiumAO.Server.Systems.NPCs;

public class NPC
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public NPCType Type { get; set; }
    public int MapId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Heading { get; set; }
    public CombatStats Stats { get; set; } = new();
    public int Level { get; set; }
    public AIBehavior Behavior { get; set; } = AIBehavior.Idle;
    public int AggroRange { get; set; } = 10;
    public bool IsAggressive { get; set; }
    public DateTime LastActionTime { get; set; } = DateTime.UtcNow;
    public int? CurrentTargetId { get; set; }
}

public enum NPCType
{
    Merchant,
    Quest,
    Guard,
    Monster,
    Boss,
    Neutral,
    Friendly
}

public enum AIBehavior
{
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    Fleeing,
    Talking
}

