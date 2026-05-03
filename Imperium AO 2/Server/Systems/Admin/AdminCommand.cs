using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Admin;

public enum AdminLevel
{
    Player = 0,
    Moderator = 1,
    GameMaster = 2,
    Administrator = 3
}

public class AdminCommand
{
    public string Name { get; set; } = string.Empty;
    public AdminLevel RequiredLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public DateTime ExecutedAt { get; set; }
    public int ExecutedBy { get; set; }
}

public class Ban
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int BannedBy { get; set; }
    public DateTime BannedAt { get; set; }
    public DateTime? UnbanAt { get; set; }
    public bool IsPermanent { get; set; }
    public bool IsActive => !UnbanAt.HasValue || DateTime.UtcNow < UnbanAt.Value;
}

public class Mute
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int MutedBy { get; set; }
    public DateTime MutedAt { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsActive => (DateTime.UtcNow - MutedAt).TotalSeconds < DurationSeconds;
}
