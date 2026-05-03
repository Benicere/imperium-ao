using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Inventory;

public class ItemDropManager : IItemDropManager
{
    private readonly Dictionary<int, ItemDrop> _droppedItems = new();
    private readonly ILogger<ItemDropManager> _logger;
    private int _dropIdCounter = 1;

    public ItemDropManager(ILogger<ItemDropManager> logger)
    {
        _logger = logger;
    }

    public void DropItem(int playerId, int itemId, int x, int y, int z)
    {
        var drop = new ItemDrop
        {
            Id = _dropIdCounter++,
            ItemId = itemId,
            X = x,
            Y = y,
            Z = z,
            Quantity = 1,
            OwnerId = playerId,
            DroppedAt = DateTime.UtcNow
        };

        _droppedItems[drop.Id] = drop;
        _logger.LogInformation($"Item {itemId} dropped at ({x}, {y}, {z})");
    }

    public void PickupItem(int playerId, int dropId)
    {
        if (_droppedItems.TryGetValue(dropId, out var drop))
        {
            if (drop.CanBePickedUpBy(playerId))
            {
                RemoveDrop(dropId);
                _logger.LogInformation($"Player {playerId} picked up item {drop.ItemId}");
            }
            else
            {
                _logger.LogWarning($"Player {playerId} cannot pick up item {dropId} (owned by {drop.OwnerId})");
            }
        }
    }

    public List<ItemDrop> GetItemsAt(int x, int y, int z)
    {
        ClearExpiredDrops();
        return _droppedItems.Values
            .Where(d => d.X == x && d.Y == y && d.Z == z && !d.IsExpired)
            .ToList();
    }

    public ItemDrop? GetDropById(int dropId)
    {
        _droppedItems.TryGetValue(dropId, out var drop);
        return drop?.IsExpired == false ? drop : null;
    }

    public void RemoveDrop(int dropId)
    {
        _droppedItems.Remove(dropId);
    }

    public void ClearExpiredDrops()
    {
        var expired = _droppedItems.Where(kvp => kvp.Value.IsExpired).Select(kvp => kvp.Key).ToList();
        foreach (var dropId in expired)
        {
            RemoveDrop(dropId);
            _logger.LogInformation($"Cleared expired drop {dropId}");
        }
    }
}

