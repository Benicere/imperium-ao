using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ImperiumAO.Server.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImperiumAO.Server.Network;

public class TcpNetworkServer : BackgroundService
{
    private readonly ServerOptions _options;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<TcpNetworkServer> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private TcpListener? _listener;
    private int _nextConnectionId = 1;

    public TcpNetworkServer(
        IOptions<ServerOptions> options,
        IConnectionManager connectionManager,
        ILogger<TcpNetworkServer> logger,
        ILoggerFactory loggerFactory)
    {
        _options = options.Value;
        _connectionManager = connectionManager;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _listener = new TcpListener(IPAddress.Any, _options.Port);
            _listener.Start(_options.MaxConnections);
            _logger.LogInformation("Game server listening on port {Port}", _options.Port);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync(stoppingToken);
                    _ = HandleClientAsync(tcpClient, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TCP network server");
            throw;
        }
        finally
        {
            _listener?.Stop();
            _logger.LogInformation("Game server stopped");
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken stoppingToken)
    {
        var connectionId = Interlocked.Increment(ref _nextConnectionId);

        _logger.LogDebug("Connection {ConnectionId}: client connected from {RemoteEndPoint}",
            connectionId, tcpClient.Client.RemoteEndPoint);

        try
        {
            var gameConnLogger = _loggerFactory.CreateLogger<GameConnection>();
            var connection = new GameConnection(connectionId, tcpClient, gameConnLogger);

            await _connectionManager.RegisterConnectionAsync(connectionId, connection);

            while (!stoppingToken.IsCancellationRequested && connection.IsConnected)
            {
                var packet = await connection.ReadPacketAsync(stoppingToken);
                if (packet == null)
                {
                    break;
                }

                // Dispatch packet to handler
                await _connectionManager.HandlePacketAsync(connectionId, packet);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling connection {ConnectionId}", connectionId);
        }
        finally
        {
            await _connectionManager.UnregisterConnectionAsync(connectionId);
            tcpClient.Dispose();
        }
    }
}
