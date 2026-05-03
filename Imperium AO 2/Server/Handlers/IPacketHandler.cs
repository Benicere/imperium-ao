using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using System;

namespace ImperiumAO.Server.Handlers;

public interface IPacketHandler
{
    ClientPacketId PacketId { get; }
    Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection);
}

