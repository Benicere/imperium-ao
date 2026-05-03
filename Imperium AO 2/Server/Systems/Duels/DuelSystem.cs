using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Duels;

public class DuelSystem : IDuelSystem
{
    private readonly Dictionary<int, DuelChallenge> _challenges = new();
    private readonly Dictionary<int, DuelMatch> _matches = new();
    private int _nextChallengeId = 1;
    private int _nextMatchId = 1;
    private readonly ILogger<DuelSystem> _logger;

    public DuelSystem(ILogger<DuelSystem> logger)
    {
        _logger = logger;
    }

    public DuelChallenge? CreateChallenge(int challengerId, string challengerName, int challengedId, string challengedName)
    {
        var challenge = new DuelChallenge
        {
            Id = _nextChallengeId++,
            ChallengerId = challengerId,
            ChallengerName = challengerName,
            ChallengedId = challengedId,
            ChallengedName = challengedName,
            Status = DuelStatus.Pending
        };

        _challenges[challenge.Id] = challenge;
        _logger.LogInformation($"Duel challenge created: {challengerName} vs {challengedName}");
        return challenge;
    }

    public DuelChallenge? GetChallenge(int challengeId)
    {
        _challenges.TryGetValue(challengeId, out var challenge);
        return challenge;
    }

    public bool AcceptChallenge(int challengeId, int characterId)
    {
        if (!_challenges.TryGetValue(challengeId, out var challenge))
        {
            return false;
        }

        if (challenge.ChallengedId != characterId)
        {
            return false;
        }

        challenge.Status = DuelStatus.Accepted;
        _logger.LogInformation($"Duel challenge {challengeId} accepted by {challenge.ChallengedName}");
        return true;
    }

    public bool RejectChallenge(int challengeId, int characterId)
    {
        if (!_challenges.TryGetValue(challengeId, out var challenge))
        {
            return false;
        }

        if (challenge.ChallengedId != characterId)
        {
            return false;
        }

        challenge.Status = DuelStatus.Rejected;
        _challenges.Remove(challengeId);
        _logger.LogInformation($"Duel challenge {challengeId} rejected by {challenge.ChallengedName}");
        return true;
    }

    public bool StartDuel(int challengeId)
    {
        if (!_challenges.TryGetValue(challengeId, out var challenge))
        {
            return false;
        }

        if (challenge.Status != DuelStatus.Accepted)
        {
            return false;
        }

        challenge.Status = DuelStatus.InProgress;
        challenge.StartedAt = DateTime.UtcNow;

        var match = new DuelMatch
        {
            Id = _nextMatchId++,
            ChallengeId = challengeId,
            Player1Id = challenge.ChallengerId,
            Player2Id = challenge.ChallengedId,
            Player1Health = 100,
            Player2Health = 100
        };

        _matches[match.Id] = match;
        _logger.LogInformation($"Duel {challengeId} started: {challenge.ChallengerName} vs {challenge.ChallengedName}");
        return true;
    }

    public bool EndDuel(int challengeId, int winnerId)
    {
        if (!_challenges.TryGetValue(challengeId, out var challenge))
        {
            return false;
        }

        challenge.Status = DuelStatus.Completed;
        challenge.EndedAt = DateTime.UtcNow;
        challenge.WinnerId = winnerId;

        _logger.LogInformation($"Duel {challengeId} ended. Winner: {winnerId}");
        return true;
    }

    public List<DuelChallenge> GetPendingChallenges(int characterId)
    {
        return _challenges.Values
            .Where(c => c.ChallengedId == characterId && c.Status == DuelStatus.Pending)
            .ToList();
    }

    public List<DuelChallenge> GetActiveDuels()
    {
        return _challenges.Values
            .Where(c => c.Status == DuelStatus.InProgress)
            .ToList();
    }
}

