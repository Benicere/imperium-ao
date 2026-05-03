using System;
using System.Threading;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;

namespace ImperiumAO.Server.Network;

public interface IGameConnection : IDisposable
{
    int ConnectionId { get; }
    bool IsConnected { get; }

    Task<ByteBuffer?> ReadPacketAsync(CancellationToken cancellationToken);
    Task SendAsync(ByteBuffer packet, CancellationToken cancellationToken);
    Task CloseAsync();
}
