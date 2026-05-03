using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Combat;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ImperiumAO.Server.Handlers;

public class DefenseHandler : IPacketHandler
{
    private readonly ICombatResolver _combatResolver;
    private readonly ILogger<DefenseHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Attack;

    public DefenseHandler(
        ICombatResolver combatResolver,
        ILogger<DefenseHandler> logger)
    {
        _combatResolver = combatResolver;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var isDefending = packet.GetByte() > 0;

            var session = _combatResolver.GetCombatSession(connectionId);
            if (session != null && isDefending)
            {
                session.DefenderStats.DefenseMultiplier = 1.5f;
                _logger.LogInformation($"Character {connectionId} is now defending");
            }
            else if (session != null && !isDefending)
            {
                session.DefenderStats.DefenseMultiplier = 1.0f;
                _logger.LogInformation($"Character {connectionId} stopped defending");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling defense packet");
        }
    }
}


