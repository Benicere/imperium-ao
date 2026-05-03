using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Guilds;

public class Guild
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int LeaderId { get; set; }
    public int MaxMembers { get; set; } = 50;
    public long GuildTreasury { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public GuildAlignment Alignment { get; set; } = GuildAlignment.Neutral;
    public List<GuildMember> Members { get; set; } = new();
    public string Motd { get; set; } = "";
    public int Level { get; set; } = 1;
}

public class GuildMember
{
    public int CharacterId { get; set; }
    public string Name { get; set; } = "";
    public GuildRank Rank { get; set; } = GuildRank.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public int ContributedGold { get; set; }
}

public enum GuildRank
{
    Leader,
    Officer,
    Member,
    Recruit
}

public enum GuildAlignment
{
    Real,
    Chaos,
    Neutral
}

