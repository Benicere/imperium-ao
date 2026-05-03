using ImperiumAO.Server.Systems.Inventory;
using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Banking;

public class BankAccount
{
    public int CharacterId { get; set; }
    public long Gold { get; set; }
    public List<Item> Items { get; set; } = new();
    public int MaxSlots { get; set; } = 50;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
}


