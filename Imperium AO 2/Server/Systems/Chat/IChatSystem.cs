using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Chat;

public interface IChatSystem
{
    void SendMessage(ChatMessage message);
    List<ChatMessage> GetMessages(ChatChannel channel, int limit = 50);
    List<ChatMessage> GetWhisperHistory(int characterId1, int characterId2, int limit = 20);
    bool FilterMessage(string message);
    void AddBlockedPlayer(int characterId, int blockedCharacterId);
    bool IsPlayerBlocked(int characterId, int otherCharacterId);
    List<int> GetBlockedPlayers(int characterId);
}

