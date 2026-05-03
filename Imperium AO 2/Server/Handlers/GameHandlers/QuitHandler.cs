using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Core;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers.GameHandlers;

public class QuitHandler : IPacketHandler
{
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<QuitHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Quit;

    public QuitHandler(
        IConnectionManager connectionManager,
        ILogger<QuitHandler> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            _logger.LogInformation("Connection {ConnectionId}: Quit request", connectionId);

            // Send disconnect confirmation
            var disconnectPacket = PacketWriter.Disconnect();
            await connection.SendAsync(disconnectPacket, default);

            // Close connection
            await connection.CloseAsync();

            // Unregister connection
            await _connectionManager.UnregisterConnectionAsync(connectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in QuitHandler");
        }
    }
}
