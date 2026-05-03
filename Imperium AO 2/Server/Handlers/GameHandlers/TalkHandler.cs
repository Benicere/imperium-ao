using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Core;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers.GameHandlers;

public class TalkHandler : IPacketHandler
{
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<TalkHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public TalkHandler(
        IConnectionManager connectionManager,
        ILogger<TalkHandler> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            packet.GetByte(); // Consume packet ID
            var message = packet.GetString();

            _logger.LogDebug("Connection {ConnectionId}: Chat message: {Message}",
                connectionId, message);

            // Broadcast chat message to all players
            var chatPacket = PacketWriter.ChatOverHead(connectionId, message);
            await _connectionManager.BroadcastAsync(chatPacket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TalkHandler");
        }
    }
}
