using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImperiumAO.Common.Entities;

namespace ImperiumAO.Server;

public interface IPlayerManager
{
    Task<Player?> LoadPlayerAsync(int playerId);
    Task SavePlayerAsync(Player player);
    Task SaveAllAsync();
    Task<IReadOnlyList<Player>> GetOnlinePlayersAsync();
}
