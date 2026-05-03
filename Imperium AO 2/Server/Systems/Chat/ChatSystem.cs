using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Chat;

public class ChatSystem : IChatSystem
{
    private readonly List<ChatMessage> _messages = new();
    private readonly Dictionary<int, HashSet<int>> _blockedPlayers = new();
    private readonly ILogger<ChatSystem> _logger;
    private readonly HashSet<string> _bannedWords = new() { "badword1", "badword2" };

    public ChatSystem(ILogger<ChatSystem> logger)
    {
        _logger = logger;
    }

    public void SendMessage(ChatMessage message)
    {
        if (IsPlayerBlocked(message.SenderId, message.ReceiverId ?? -1))
        {
            _logger.LogWarning($"Message from blocked player {message.SenderId}");
            return;
        }

        if (!FilterMessage(message.Message))
        {
            _logger.LogWarning($"Message contains banned words from {message.SenderId}");
            return;
        }

        _messages.Add(message);
        _logger.LogInformation($"[{message.Channel}] {message.SenderName}: {message.Message}");
    }

    public List<ChatMessage> GetMessages(ChatChannel channel, int limit = 50)
    {
        return _messages
            .Where(m => m.Channel == channel)
            .TakeLast(limit)
            .ToList();
    }

    public List<ChatMessage> GetWhisperHistory(int characterId1, int characterId2, int limit = 20)
    {
        return _messages
            .Where(m => m.Channel == ChatChannel.Whisper &&
                        ((m.SenderId == characterId1 && m.ReceiverId == characterId2) ||
                         (m.SenderId == characterId2 && m.ReceiverId == characterId1)))
            .TakeLast(limit)
            .ToList();
    }

    public bool FilterMessage(string message)
    {
        var lowerMessage = message.ToLower();
        return !_bannedWords.Any(word => lowerMessage.Contains(word));
    }

    public void AddBlockedPlayer(int characterId, int blockedCharacterId)
    {
        if (!_blockedPlayers.ContainsKey(characterId))
        {
            _blockedPlayers[characterId] = new();
        }

        _blockedPlayers[characterId].Add(blockedCharacterId);
        _logger.LogInformation($"Character {characterId} blocked player {blockedCharacterId}");
    }

    public bool IsPlayerBlocked(int characterId, int otherCharacterId)
    {
        if (!_blockedPlayers.TryGetValue(characterId, out var blocked))
        {
            return false;
        }

        return blocked.Contains(otherCharacterId);
    }

    public List<int> GetBlockedPlayers(int characterId)
    {
        if (!_blockedPlayers.TryGetValue(characterId, out var blocked))
        {
            return new();
        }

        return blocked.ToList();
    }
}


