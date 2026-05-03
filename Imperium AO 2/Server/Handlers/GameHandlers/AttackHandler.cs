using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Core;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers.GameHandlers;

public class AttackHandler : IPacketHandler
{
    private readonly ICombatSystem _combatSystem;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<AttackHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Attack;

    public AttackHandler(
        ICombatSystem combatSystem,
        IConnectionManager connectionManager,
        ILogger<AttackHandler> logger)
    {
        _combatSystem = combatSystem;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            packet.GetByte(); // Consume packet ID

            _logger.LogDebug("Connection {ConnectionId}: Attack request", connectionId);

            // TODO: Get current character and target from connection context
            // For now, just log the attack
            _logger.LogInformation("Connection {ConnectionId}: Attack action processed", connectionId);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AttackHandler");
        }
    }
}
