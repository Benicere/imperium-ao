using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;

namespace ImperiumAO.Server.Core;

public interface IConnectionManager
{
    Task RegisterConnectionAsync(int connectionId, IGameConnection connection);
    Task UnregisterConnectionAsync(int connectionId);
    Task HandlePacketAsync(int connectionId, ByteBuffer packet);
    Task SendToConnectionAsync(int connectionId, ByteBuffer packet);
    Task BroadcastAsync(ByteBuffer packet);
    IGameConnection? GetConnection(int connectionId);
}
