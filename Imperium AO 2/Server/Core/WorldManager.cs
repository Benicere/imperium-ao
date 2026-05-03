using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server;

public class WorldManager : IWorldManager
{
    private readonly ILogger<WorldManager> _logger;
    private bool _isRunning;
    private Task? _updateTask;

    public WorldManager(ILogger<WorldManager> logger)
    {
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("World Manager initializing");
        _isRunning = true;

        _updateTask = UpdateLoopAsync(cancellationToken);

        await Task.CompletedTask;
    }

    public async Task ShutdownAsync()
    {
        _logger.LogInformation("World Manager shutting down");
        _isRunning = false;
        if (_updateTask is not null)
        {
            await _updateTask;
        }
    }

    public Task UpdateAsync(float deltaTime)
    {
        return Task.CompletedTask;
    }

    private async Task UpdateLoopAsync(CancellationToken cancellationToken)
    {
        const int targetFrameRate = 60;
        const float targetDeltaTime = 1f / targetFrameRate;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                var deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                await UpdateAsync(deltaTime);

                var sleepTime = targetDeltaTime - deltaTime;
                if (sleepTime > 0)
                {
                    await Task.Delay((int)(sleepTime * 1000), cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Update loop cancelled");
        }
    }
}
