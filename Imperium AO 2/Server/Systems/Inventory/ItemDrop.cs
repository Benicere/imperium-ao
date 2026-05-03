using System;
namespace ImperiumAO.Server.Systems.Inventory;

public class ItemDrop
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Quantity { get; set; }
    public int OwnerId { get; set; }
    public DateTime DroppedAt { get; set; }
    public int ExpirationSeconds { get; set; } = 300;
    public bool IsExpired => (DateTime.UtcNow - DroppedAt).TotalSeconds > ExpirationSeconds;
    public bool CanBePickedUpBy(int playerId) => OwnerId == 0 || OwnerId == playerId || IsExpired;
}

