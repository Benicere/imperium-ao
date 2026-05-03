using ImperiumAO.Common.Network;
using ImperiumAO.Server.Systems.Chat;
using ImperiumAO.Server.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ImperiumAO.Server.Handlers;

public class TalkHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<TalkHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public TalkHandler(
        IChatSystem chatSystem,
        ILogger<TalkHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var message = packet.GetString();

            var chatMessage = new ChatMessage
            {
                SenderId = connectionId,
                Channel = ChatChannel.Say,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            _chatSystem.SendMessage(chatMessage);

            var responsePacket = new ByteBuffer();
            responsePacket.InitializeWriter();
            responsePacket.PutByte((byte)ServerPacketId.ChatOverHead);
            responsePacket.PutByte((byte)connectionId);
            responsePacket.PutString(message);

            await connection.SendAsync(responsePacket, default);

            _logger.LogInformation($"Player {connectionId}: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling talk packet");
        }
    }
}

