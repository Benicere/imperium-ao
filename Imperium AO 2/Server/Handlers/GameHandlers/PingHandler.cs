using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers.GameHandlers;

public class PingHandler : IPacketHandler
{
    private readonly ILogger<PingHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Ping;

    public PingHandler(ILogger<PingHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            _logger.LogDebug("Connection {ConnectionId}: Ping received", connectionId);
            var pongPacket = PacketWriter.Pong();
            await connection.SendAsync(pongPacket, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PingHandler");
        }
    }
}
