using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Chat;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers;

public class WhisperHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<WhisperHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public WhisperHandler(IChatSystem chatSystem, ILogger<WhisperHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var targetName = packet.GetString();
            var message = packet.GetString();

            var msg = new ChatMessage
            {
                SenderId = connectionId,
                Channel = ChatChannel.Whisper,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            _chatSystem.SendMessage(msg);
            _logger.LogInformation($"Whisper from {connectionId} to {targetName}: {message}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling whisper packet");
        }
    }
}

public class GlobalChatHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<GlobalChatHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public GlobalChatHandler(IChatSystem chatSystem, ILogger<GlobalChatHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var message = packet.GetString();

            var msg = new ChatMessage
            {
                SenderId = connectionId,
                Channel = ChatChannel.Global,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            _chatSystem.SendMessage(msg);
            _logger.LogInformation($"Global chat from {connectionId}: {message}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling global chat packet");
        }
    }
}

public class PartyChatHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<PartyChatHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public PartyChatHandler(IChatSystem chatSystem, ILogger<PartyChatHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var message = packet.GetString();

            var msg = new ChatMessage
            {
                SenderId = connectionId,
                Channel = ChatChannel.Party,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            _chatSystem.SendMessage(msg);
            _logger.LogInformation($"Party chat from {connectionId}: {message}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling party chat packet");
        }
    }
}

public class GuildChatHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<GuildChatHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public GuildChatHandler(IChatSystem chatSystem, ILogger<GuildChatHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var message = packet.GetString();

            var msg = new ChatMessage
            {
                SenderId = connectionId,
                Channel = ChatChannel.Guild,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            _chatSystem.SendMessage(msg);
            _logger.LogInformation($"Guild chat from {connectionId}: {message}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling guild chat packet");
        }
    }
}

public class YellChatHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<YellChatHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public YellChatHandler(IChatSystem chatSystem, ILogger<YellChatHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var message = packet.GetString();

            var msg = new ChatMessage
            {
                SenderId = connectionId,
                Channel = ChatChannel.Yell,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            _chatSystem.SendMessage(msg);
            _logger.LogInformation($"Yell from {connectionId}: {message}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling yell chat packet");
        }
    }
}

public class BlockPlayerHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<BlockPlayerHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public BlockPlayerHandler(IChatSystem chatSystem, ILogger<BlockPlayerHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var blockedPlayerId = packet.GetInteger();
            _logger.LogInformation($"Player {connectionId} blocked {blockedPlayerId}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling block player packet");
        }
    }
}

public class UnblockPlayerHandler : IPacketHandler
{
    private readonly IChatSystem _chatSystem;
    private readonly ILogger<UnblockPlayerHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Talk;

    public UnblockPlayerHandler(IChatSystem chatSystem, ILogger<UnblockPlayerHandler> logger)
    {
        _chatSystem = chatSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var unblockedPlayerId = packet.GetInteger();
            _logger.LogInformation($"Player {connectionId} unblocked {unblockedPlayerId}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling unblock player packet");
        }
    }
}


