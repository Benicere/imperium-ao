using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImperiumAO.Server;

public interface IWorldManager
{
    Task InitializeAsync(CancellationToken cancellationToken);
    Task ShutdownAsync();
    Task UpdateAsync(float deltaTime);
}
