using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperiumAO.Common.Entities;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server;

public class PlayerManager : IPlayerManager
{
    private readonly ILogger<PlayerManager> _logger;
    private readonly Dictionary<int, Player> _players = new();

    public PlayerManager(ILogger<PlayerManager> logger)
    {
        _logger = logger;
    }

    public Task<Player?> LoadPlayerAsync(int playerId)
    {
        _logger.LogDebug("Loading player {PlayerId}", playerId);

        if (_players.TryGetValue(playerId, out var player))
        {
            return Task.FromResult<Player?>(player);
        }

        return Task.FromResult<Player?>(null);
    }

    public Task SavePlayerAsync(Player player)
    {
        _logger.LogDebug("Saving player {PlayerId}: {PlayerName}", player.Id, player.Name);
        _players[player.Id] = player;
        return Task.CompletedTask;
    }

    public Task SaveAllAsync()
    {
        _logger.LogInformation("Saving {Count} players", _players.Count);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Player>> GetOnlinePlayersAsync()
    {
        return Task.FromResult<IReadOnlyList<Player>>(_players.Values.ToList());
    }
}
