using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Combat;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ImperiumAO.Server.Handlers;

public class AttackHandler : IPacketHandler
{
    private readonly ICombatCalculator _combatCalculator;
    private readonly ICombatResolver _combatResolver;
    private readonly ILogger<AttackHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Attack;

    public AttackHandler(
        ICombatCalculator combatCalculator,
        ICombatResolver combatResolver,
        ILogger<AttackHandler> logger)
    {
        _combatCalculator = combatCalculator;
        _combatResolver = combatResolver;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var targetId = packet.GetInteger();
            _combatResolver.StartCombat(connectionId, targetId);
            _logger.LogInformation($"Attack: {connectionId} -> {targetId}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling attack packet");
        }
    }
}


