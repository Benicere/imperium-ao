using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Statistics;

public class PlayerStatistics
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int TotalPlayTime { get; set; }
    public int KillCount { get; set; }
    public int DeathCount { get; set; }
    public int QuestsCompleted { get; set; }
    public int GoldsEarned { get; set; }
    public int GoldsSpent { get; set; }
    public int ItemsCrafted { get; set; }
    public int ItemsTraded { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime FirstLogin { get; set; }
    public int LongestSessionSeconds { get; set; }
    public int BossesDefeated { get; set; }
    public int DuelsWon { get; set; }
    public int DuelsLost { get; set; }

    public double KDRatio => DeathCount > 0 ? (double)KillCount / DeathCount : KillCount;
    public double DuelWinRate => DuelsWon + DuelsLost > 0 ? (double)DuelsWon / (DuelsWon + DuelsLost) : 0;
    public int NetGold => GoldsEarned - GoldsSpent;
}

public class GameStatistics
{
    public int TotalPlayersOnline { get; set; }
    public int TotalPlayersRegistered { get; set; }
    public int TotalQuestsCompleted { get; set; }
    public int TotalBossDefeats { get; set; }
    public int TotalItemsCrafted { get; set; }
    public int TotalTradesCompleted { get; set; }
    public int TotalCombatEncounters { get; set; }
    public DateTime RecordedAt { get; set; }
    public int AveragePlayersPerHour { get; set; }
    public int PeakPlayersToday { get; set; }
}

