using ImperiumAO.Server.Systems.Inventory;
using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Trading;

public class TradeOffer
{
    public int Id { get; set; }
    public int InitiatorId { get; set; }
    public int TargetId { get; set; }
    public List<Item> InitiatorItems { get; set; } = new();
    public List<Item> TargetItems { get; set; } = new();
    public int InitiatorGold { get; set; }
    public int TargetGold { get; set; }
    public bool InitiatorAccepted { get; set; }
    public bool TargetAccepted { get; set; }
    public TradeStatus Status { get; set; } = TradeStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TradeStatus
{
    Pending,
    Accepted,
    Rejected,
    Completed,
    Cancelled
}

