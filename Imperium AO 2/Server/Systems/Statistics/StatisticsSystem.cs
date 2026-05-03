using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Systems.Statistics;

public class StatisticsSystem : IStatisticsSystem
{
    private readonly Dictionary<int, PlayerStatistics> _playerStats = new();
    private readonly Dictionary<int, DateTime> _playerLoginTimes = new();
    private readonly ILogger<StatisticsSystem> _logger;

    public StatisticsSystem(ILogger<StatisticsSystem> logger)
    {
        _logger = logger;
    }

    public void RecordPlayerLogin(int playerId, string playerName)
    {
        if (!_playerStats.ContainsKey(playerId))
        {
            _playerStats[playerId] = new()
            {
                PlayerId = playerId,
                PlayerName = playerName,
                FirstLogin = DateTime.UtcNow
            };
        }

        _playerLoginTimes[playerId] = DateTime.UtcNow;
        _playerStats[playerId].LastLogin = DateTime.UtcNow;
        _logger.LogInformation($"Player {playerName} logged in");
    }

    public void RecordPlayerLogout(int playerId)
    {
        if (_playerLoginTimes.TryGetValue(playerId, out var loginTime))
        {
            var sessionLength = (int)(DateTime.UtcNow - loginTime).TotalSeconds;
            _playerLoginTimes.Remove(playerId);

            if (_playerStats.TryGetValue(playerId, out var stats))
            {
                stats.TotalPlayTime += sessionLength;
                if (sessionLength > stats.LongestSessionSeconds)
                {
                    stats.LongestSessionSeconds = sessionLength;
                }
            }
        }
    }

    public void RecordKill(int killerId, int victimId)
    {
        if (_playerStats.TryGetValue(killerId, out var killerStats))
        {
            killerStats.KillCount++;
        }

        if (_playerStats.TryGetValue(victimId, out var victimStats))
        {
            victimStats.DeathCount++;
        }

        _logger.LogInformation($"Kill recorded: {killerId} killed {victimId}");
    }

    public void RecordDeath(int playerId)
    {
        if (_playerStats.TryGetValue(playerId, out var stats))
        {
            stats.DeathCount++;
        }
    }

    public void RecordQuestCompletion(int playerId)
    {
        if (_playerStats.TryGetValue(playerId, out var stats))
        {
            stats.QuestsCompleted++;
        }
    }

    public void RecordItemCraft(int playerId)
    {
        if (_playerStats.TryGetValue(playerId, out var stats))
        {
            stats.ItemsCrafted++;
        }
    }

    public void RecordTrade(int playerId1, int playerId2)
    {
        if (_playerStats.TryGetValue(playerId1, out var stats1))
        {
            stats1.ItemsTraded++;
        }

        if (_playerStats.TryGetValue(playerId2, out var stats2))
        {
            stats2.ItemsTraded++;
        }
    }

    public void RecordGoldEarned(int playerId, int amount)
    {
        if (_playerStats.TryGetValue(playerId, out var stats))
        {
            stats.GoldsEarned += amount;
        }
    }

    public void RecordGoldSpent(int playerId, int amount)
    {
        if (_playerStats.TryGetValue(playerId, out var stats))
        {
            stats.GoldsSpent += amount;
        }
    }

    public PlayerStatistics? GetPlayerStatistics(int playerId)
    {
        _playerStats.TryGetValue(playerId, out var stats);
        return stats;
    }

    public GameStatistics GetGameStatistics()
    {
        return new()
        {
            TotalPlayersOnline = _playerLoginTimes.Count,
            TotalPlayersRegistered = _playerStats.Count,
            TotalQuestsCompleted = _playerStats.Values.Sum(s => s.QuestsCompleted),
            TotalItemsCrafted = _playerStats.Values.Sum(s => s.ItemsCrafted),
            TotalTradesCompleted = _playerStats.Values.Sum(s => s.ItemsTraded) / 2,
            RecordedAt = DateTime.UtcNow
        };
    }

    public List<PlayerStatistics> GetTopKillers(int count)
    {
        return _playerStats.Values.OrderByDescending(s => s.KillCount).Take(count).ToList();
    }

    public List<PlayerStatistics> GetTopLevelPlayers(int count)
    {
        return _playerStats.Values.OrderByDescending(s => s.QuestsCompleted).Take(count).ToList();
    }

    public List<PlayerStatistics> GetMostActivePlayers(int count)
    {
        return _playerStats.Values.OrderByDescending(s => s.TotalPlayTime).Take(count).ToList();
    }
}

