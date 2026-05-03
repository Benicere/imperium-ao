using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers;

public interface IPacketDispatcher
{
    Task DispatchAsync(int connectionId, ByteBuffer packet, IGameConnection connection);
}

public class PacketDispatcher : IPacketDispatcher
{
    private readonly ILogger<PacketDispatcher> _logger;
    private readonly Dictionary<ClientPacketId, IPacketHandler> _handlers;

    public PacketDispatcher(IEnumerable<IPacketHandler> handlers, ILogger<PacketDispatcher> logger)
    {
        _logger = logger;
        _handlers = handlers.ToDictionary(h => h.PacketId);
    }

    public async Task DispatchAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            if (packet.GetCurrentPos() >= packet.GetLastPos() + 1)
            {
                _logger.LogWarning("Connection {ConnectionId}: empty packet", connectionId);
                return;
            }

            byte packetIdByte = packet.GetByte();
            var packetId = (ClientPacketId)packetIdByte;

            if (_handlers.TryGetValue(packetId, out var handler))
            {
                packet.GetVoid(-1); // Reset to start for handler
                await handler.HandleAsync(connectionId, packet, connection);
            }
            else
            {
                _logger.LogDebug("Connection {ConnectionId}: no handler for packet {PacketId}",
                    connectionId, packetId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection {ConnectionId}: error dispatching packet",
                connectionId);
        }
    }
}
