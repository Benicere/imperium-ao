using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Duels;

public class DuelChallenge
{
    public int Id { get; set; }
    public int ChallengerId { get; set; }
    public string ChallengerName { get; set; } = "";
    public int ChallengedId { get; set; }
    public string ChallengedName { get; set; } = "";
    public DuelStatus Status { get; set; } = DuelStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int? WinnerId { get; set; }
}

public class DuelMatch
{
    public int Id { get; set; }
    public int ChallengeId { get; set; }
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }
    public int? WinnerId { get; set; }
    public int Player1Health { get; set; }
    public int Player2Health { get; set; }
}

public enum DuelStatus
{
    Pending,
    Accepted,
    InProgress,
    Completed,
    Cancelled,
    Rejected
}

