using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Systems.PvP;

public class PvPSystem : IPvPSystem
{
    private readonly Dictionary<int, PvPFlag> _playerFlags = new();
    private readonly ILogger<PvPSystem> _logger;

    public PvPSystem(ILogger<PvPSystem> logger)
    {
        _logger = logger;
    }

    public void FlagForPvP(int playerId)
    {
        if (!_playerFlags.ContainsKey(playerId))
        {
            _playerFlags[playerId] = new() { PlayerId = playerId };
        }

        var flag = _playerFlags[playerId];
        if (flag.Status == PvPStatus.Safe)
        {
            flag.Status = PvPStatus.Flagged;
            flag.FlaggedAt = DateTime.UtcNow;
            _logger.LogInformation($"Player {playerId} flagged for PvP");
        }
    }

    public void UnflagPlayer(int playerId)
    {
        if (_playerFlags.TryGetValue(playerId, out var flag))
        {
            if (flag.ShouldUnflag())
            {
                flag.Status = PvPStatus.Safe;
                flag.KilledPlayers.Clear();
                _logger.LogInformation($"Player {playerId} unflagged");
            }
        }
    }

    public void ReportKill(int killerId, int victimId)
    {
        if (!_playerFlags.ContainsKey(killerId))
        {
            _playerFlags[killerId] = new() { PlayerId = killerId };
        }

        var killerFlag = _playerFlags[killerId];
        killerFlag.IncrementKills();
        killerFlag.KilledPlayers.Add(victimId);

        if (killerFlag.Notoriety >= 30)
        {
            killerFlag.Status = PvPStatus.Killer;
        }
        else if (killerFlag.Notoriety >= 20)
        {
            killerFlag.Status = PvPStatus.Criminal;
        }

        if (_playerFlags.TryGetValue(victimId, out var victimFlag))
        {
            victimFlag.IncrementDeaths();
        }

        _logger.LogInformation($"Player {killerId} killed {victimId} (Notoriety: {killerFlag.Notoriety})");
    }

    public void ReportCriminalAct(int playerId)
    {
        if (!_playerFlags.ContainsKey(playerId))
        {
            _playerFlags[playerId] = new() { PlayerId = playerId };
        }

        var flag = _playerFlags[playerId];
        flag.Status = PvPStatus.Criminal;
        flag.FlaggedAt = DateTime.UtcNow;
        flag.Notoriety = Math.Min(100, flag.Notoriety + 15);
        _logger.LogInformation($"Player {playerId} reported as criminal (Notoriety: {flag.Notoriety})");
    }

    public PvPFlag? GetPvPStatus(int playerId)
    {
        _playerFlags.TryGetValue(playerId, out var flag);
        return flag;
    }

    public bool CanAttack(int attackerId, int defenderId)
    {
        var attackerFlag = GetPvPStatus(attackerId);
        var defenderFlag = GetPvPStatus(defenderId);

        if (attackerFlag?.Status == PvPStatus.Flagged || defenderFlag?.Status == PvPStatus.Flagged)
            return true;

        if (attackerFlag?.Status == PvPStatus.Criminal || defenderFlag?.Status == PvPStatus.Criminal)
            return true;

        if (attackerFlag?.Status == PvPStatus.Killer)
            return true;

        return false;
    }

    public void UpdatePvPFlags()
    {
        foreach (var flag in _playerFlags.Values)
        {
            if (flag.ShouldUnflag())
            {
                UnflagPlayer(flag.PlayerId);
            }
        }
    }

    public int GetPlayerNotoriety(int playerId)
    {
        var flag = GetPvPStatus(playerId);
        return flag?.Notoriety ?? 0;
    }

    public List<int> GetPlayersWantedFor(int playerId)
    {
        var wantedPlayers = new List<int>();
        foreach (var flag in _playerFlags.Values)
        {
            if (flag.KilledPlayers.Contains(playerId) && flag.Status == PvPStatus.Wanted)
            {
                wantedPlayers.Add(flag.PlayerId);
            }
        }
        return wantedPlayers;
    }
}

