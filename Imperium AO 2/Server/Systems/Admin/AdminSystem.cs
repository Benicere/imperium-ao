using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Admin;

public class AdminSystem : IAdminSystem
{
    private readonly Dictionary<int, AdminLevel> _adminLevels = new();
    private readonly Dictionary<int, Ban> _bans = new();
    private readonly Dictionary<int, Mute> _mutes = new();
    private readonly List<AdminCommand> _commandLog = new();
    private readonly ILogger<AdminSystem> _logger;
    private int _banIdCounter = 1;
    private int _muteIdCounter = 1;

    public AdminSystem(ILogger<AdminSystem> logger)
    {
        _logger = logger;
    }

    public void BanPlayer(int playerId, string reason, int? durationSeconds = null)
    {
        var ban = new Ban
        {
            Id = _banIdCounter++,
            PlayerId = playerId,
            Reason = reason,
            BannedAt = DateTime.UtcNow,
            IsPermanent = durationSeconds == null,
            UnbanAt = durationSeconds.HasValue ? DateTime.UtcNow.AddSeconds(durationSeconds.Value) : null
        };

        _bans[playerId] = ban;
        _logger.LogInformation($"Player {playerId} banned: {reason}");
    }

    public void UnbanPlayer(int playerId)
    {
        if (_bans.TryGetValue(playerId, out var ban))
        {
            _bans.Remove(playerId);
            _logger.LogInformation($"Player {playerId} unbanned");
        }
    }

    public void MutePlayer(int playerId, string reason, int durationSeconds)
    {
        var mute = new Mute
        {
            Id = _muteIdCounter++,
            PlayerId = playerId,
            Reason = reason,
            MutedAt = DateTime.UtcNow,
            DurationSeconds = durationSeconds
        };

        _mutes[playerId] = mute;
        _logger.LogInformation($"Player {playerId} muted for {durationSeconds}s: {reason}");
    }

    public void UnmutePlayer(int playerId)
    {
        if (_mutes.TryGetValue(playerId, out var mute))
        {
            _mutes.Remove(playerId);
            _logger.LogInformation($"Player {playerId} unmuted");
        }
    }

    public bool IsPlayerBanned(int playerId)
    {
        if (_bans.TryGetValue(playerId, out var ban))
        {
            return ban.IsActive;
        }
        return false;
    }

    public bool IsPlayerMuted(int playerId)
    {
        if (_mutes.TryGetValue(playerId, out var mute))
        {
            return mute.IsActive;
        }
        return false;
    }

    public void SetAdminLevel(int playerId, AdminLevel level)
    {
        _adminLevels[playerId] = level;
        _logger.LogInformation($"Player {playerId} set to admin level {level}");
    }

    public AdminLevel GetAdminLevel(int playerId)
    {
        if (_adminLevels.TryGetValue(playerId, out var level))
        {
            return level;
        }
        return AdminLevel.Player;
    }

    public void ExecuteCommand(int adminId, AdminCommand command)
    {
        var adminLevel = GetAdminLevel(adminId);
        if (adminLevel >= command.RequiredLevel)
        {
            command.ExecutedAt = DateTime.UtcNow;
            command.ExecutedBy = adminId;
            _commandLog.Add(command);
            _logger.LogInformation($"Admin {adminId} executed command: {command.Name}");
        }
        else
        {
            _logger.LogWarning($"Admin {adminId} tried to execute command {command.Name} without permission");
        }
    }

    public void WarnPlayer(int playerId, string reason)
    {
        _logger.LogInformation($"Player {playerId} warned: {reason}");
    }

    public List<Ban> GetActiveBans()
    {
        return _bans.Values.Where(b => b.IsActive).ToList();
    }

    public List<Mute> GetActiveMutes()
    {
        return _mutes.Values.Where(m => m.IsActive).ToList();
    }

    public void KickPlayer(int playerId)
    {
        _logger.LogInformation($"Player {playerId} kicked from server");
    }
}


