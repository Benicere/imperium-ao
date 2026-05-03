using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Chat;

public class ChatMessage
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = "";
    public string Message { get; set; } = "";
    public ChatChannel Channel { get; set; } = ChatChannel.Say;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int? ReceiverId { get; set; }
}

public enum ChatChannel
{
    Say,
    Yell,
    Whisper,
    Party,
    Guild,
    Global,
    System
}

