using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Parties;

public class Party
{
    public int Id { get; set; }
    public int LeaderId { get; set; }
    public List<PartyMember> Members { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsShareExpEnabled { get; set; } = true;
    public bool IsShareLootEnabled { get; set; } = true;
    public int MaxMembers { get; set; } = 5;
}

public class PartyMember
{
    public int CharacterId { get; set; }
    public string Name { get; set; } = "";
    public int Level { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

