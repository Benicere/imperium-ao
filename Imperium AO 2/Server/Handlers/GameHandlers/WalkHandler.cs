using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Core;
using ImperiumAO.Server.Map;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers.GameHandlers;

public class WalkHandler : IPacketHandler
{
    private readonly IMapManager _mapManager;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<WalkHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Walk;

    public WalkHandler(
        IMapManager mapManager,
        IConnectionManager connectionManager,
        ILogger<WalkHandler> logger)
    {
        _mapManager = mapManager;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            packet.GetByte(); // Consume packet ID
            var direction = packet.GetByte();

            _logger.LogDebug("Connection {ConnectionId}: Walk direction: {Direction}",
                connectionId, direction);

            // TODO: Get current character from connection context
            // For now, just acknowledge the movement
            var posUpdatePacket = PacketWriter.PosUpdate(50, 50);
            await connection.SendAsync(posUpdatePacket, default);

            // Broadcast movement to nearby players
            var movePacket = PacketWriter.CharacterMove(connectionId, 50, 50);
            await _connectionManager.BroadcastAsync(movePacket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in WalkHandler");
        }
    }
}
