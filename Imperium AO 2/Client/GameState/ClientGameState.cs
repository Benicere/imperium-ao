using System;
using System.Collections.Generic;

namespace ImperiumAO.Client.GameState;

public class ClientGameState
{
    public bool IsLoggedIn { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = "";
    public int PlayerX { get; set; }
    public int PlayerY { get; set; }
    public int CurrentMap { get; set; }
    public int PlayerHealth { get; set; }
    public int PlayerMaxHealth { get; set; } = 100;
    public int PlayerMana { get; set; }
    public int PlayerMaxMana { get; set; } = 50;
    public int PlayerGold { get; set; }
    public string LastError { get; set; } = "";
    public DateTime LastPongTime { get; set; }

    private readonly List<string> _chatMessages = new();

    public IReadOnlyList<string> ChatMessages => _chatMessages.AsReadOnly();

    public void AddChatMessage(string message)
    {
        _chatMessages.Add(message);
        if (_chatMessages.Count > 50)
            _chatMessages.RemoveAt(0);
    }

    public void ClearChat()
    {
        _chatMessages.Clear();
    }
}
