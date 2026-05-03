using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server;

public class GameServer
{
    private readonly ILogger<GameServer> _logger;
    private readonly IWorldManager _worldManager;
    private readonly IPlayerManager _playerManager;
    private CancellationTokenSource? _cancellationTokenSource;

    public GameServer(
        ILogger<GameServer> logger,
        IWorldManager worldManager,
        IPlayerManager playerManager)
    {
        _logger = logger;
        _worldManager = worldManager;
        _playerManager = playerManager;
    }

    public async Task StartAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            _logger.LogInformation("Initializing game world");
            await _worldManager.InitializeAsync(token);

            _logger.LogInformation("Game server is running. Press Ctrl+C to stop.");

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                _cancellationTokenSource.Cancel();
            };

            await Task.Delay(Timeout.Infinite, token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Server shutdown initiated");
        }
        finally
        {
            await ShutdownAsync();
        }
    }

    private async Task ShutdownAsync()
    {
        _logger.LogInformation("Shutting down game server");
        try
        {
            await _worldManager.ShutdownAsync();
            await _playerManager.SaveAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during shutdown");
        }
    }
}
