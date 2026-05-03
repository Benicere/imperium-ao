using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.PvP;

public enum PvPStatus
{
    Safe,
    Flagged,
    Criminal,
    Wanted,
    Killer
}

public class PvPFlag
{
    public int PlayerId { get; set; }
    public PvPStatus Status { get; set; }
    public DateTime FlaggedAt { get; set; }
    public int KillCount { get; set; }
    public int DeathCount { get; set; }
    public int Notoriety { get; set; }
    public List<int> KilledPlayers { get; set; } = new();
    public bool CanBeAttackedByAnyone { get; set; }

    public bool IsFlagged => Status != PvPStatus.Safe;
    public int HoursUntilUnflag => IsFlagged ? (int)(DateTime.UtcNow - FlaggedAt).TotalHours : 0;

    public void IncrementKills()
    {
        KillCount++;
        Notoriety = Math.Min(100, Notoriety + 10);
    }

    public void IncrementDeaths()
    {
        DeathCount++;
    }

    public void ReduceNotoriety(int amount)
    {
        Notoriety = Math.Max(0, Notoriety - amount);
    }

    public bool ShouldUnflag()
    {
        if (Status == PvPStatus.Safe) return false;
        if (Status == PvPStatus.Flagged) return HoursUntilUnflag >= 2;
        if (Status == PvPStatus.Criminal) return HoursUntilUnflag >= 4;
        return Status == PvPStatus.Wanted && HoursUntilUnflag >= 6;
    }
}

