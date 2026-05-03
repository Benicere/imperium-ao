using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Handlers;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Core;

public class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<int, IGameConnection> _connections;
    private readonly IPacketDispatcher _dispatcher;
    private readonly ILogger<ConnectionManager> _logger;

    public ConnectionManager(IPacketDispatcher dispatcher, ILogger<ConnectionManager> logger)
    {
        _connections = new();
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public Task RegisterConnectionAsync(int connectionId, IGameConnection connection)
    {
        if (_connections.TryAdd(connectionId, connection))
        {
            _logger.LogInformation("Connection {ConnectionId}: registered. Total: {Total}",
                connectionId, _connections.Count);
        }
        return Task.CompletedTask;
    }

    public Task UnregisterConnectionAsync(int connectionId)
    {
        if (_connections.TryRemove(connectionId, out var connection))
        {
            connection?.Dispose();
            _logger.LogInformation("Connection {ConnectionId}: unregistered. Total: {Total}",
                connectionId, _connections.Count);
        }
        return Task.CompletedTask;
    }

    public async Task HandlePacketAsync(int connectionId, ByteBuffer packet)
    {
        if (_connections.TryGetValue(connectionId, out var connection))
        {
            await _dispatcher.DispatchAsync(connectionId, packet, connection);
        }
    }

    public async Task SendToConnectionAsync(int connectionId, ByteBuffer packet)
    {
        if (_connections.TryGetValue(connectionId, out var connection))
        {
            await connection.SendAsync(packet, CancellationToken.None);
        }
    }

    public async Task BroadcastAsync(ByteBuffer packet)
    {
        var tasks = _connections.Values.Select(conn =>
            conn.SendAsync(packet, CancellationToken.None));
        await Task.WhenAll(tasks);
    }

    public IGameConnection? GetConnection(int connectionId)
    {
        _connections.TryGetValue(connectionId, out var connection);
        return connection;
    }
}
