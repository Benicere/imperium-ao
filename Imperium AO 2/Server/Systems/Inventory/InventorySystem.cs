using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Inventory;

public class InventorySystem : IInventorySystem
{
    private const int MaxInventorySlots = 20;
    private const int MaxInventoryWeight = 500;

    private readonly Dictionary<int, List<Item>> _characterInventories = new();
    private readonly ILogger<InventorySystem> _logger;

    public InventorySystem(ILogger<InventorySystem> logger)
    {
        _logger = logger;
    }

    public bool AddItem(int characterId, Item item)
    {
        if (!HasSpace(characterId))
        {
            _logger.LogWarning($"Character {characterId} inventory is full");
            return false;
        }

        if (!_characterInventories.ContainsKey(characterId))
        {
            _characterInventories[characterId] = new();
        }

        var inventory = _characterInventories[characterId];

        var existingItem = inventory.FirstOrDefault(i => i.Id == item.Id && i.Quantity < i.MaxStackSize);
        if (existingItem != null && item.Quantity > 0)
        {
            var canAdd = existingItem.MaxStackSize - existingItem.Quantity;
            if (canAdd >= item.Quantity)
            {
                existingItem.Quantity += item.Quantity;
                _logger.LogInformation($"Added {item.Quantity} of {item.Name} to character {characterId} inventory");
                return true;
            }
            else if (canAdd > 0)
            {
                existingItem.Quantity = existingItem.MaxStackSize;
                item.Quantity -= canAdd;
            }
        }

        if (inventory.Count >= MaxInventorySlots)
        {
            _logger.LogWarning($"Character {characterId} inventory slots are full");
            return false;
        }

        inventory.Add(item);
        _logger.LogInformation($"Added {item.Name} to character {characterId} inventory");
        return true;
    }

    public bool RemoveItem(int characterId, int itemId, int quantity = 1)
    {
        if (!_characterInventories.TryGetValue(characterId, out var inventory))
        {
            return false;
        }

        var item = inventory.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return false;
        }

        if (item.Quantity >= quantity)
        {
            item.Quantity -= quantity;
            if (item.Quantity == 0)
            {
                inventory.Remove(item);
            }
            _logger.LogInformation($"Removed {quantity} of item {itemId} from character {characterId}");
            return true;
        }

        return false;
    }

    public Item? GetItem(int characterId, int itemId)
    {
        if (!_characterInventories.TryGetValue(characterId, out var inventory))
        {
            return null;
        }

        return inventory.FirstOrDefault(i => i.Id == itemId);
    }

    public List<Item> GetInventory(int characterId)
    {
        if (!_characterInventories.TryGetValue(characterId, out var inventory))
        {
            return new();
        }

        return inventory.ToList();
    }

    public bool UseItem(int characterId, int itemId)
    {
        var item = GetItem(characterId, itemId);
        if (item == null)
        {
            _logger.LogWarning($"Character {characterId} doesn't have item {itemId}");
            return false;
        }

        _logger.LogInformation($"Character {characterId} used {item.Name}");
        return RemoveItem(characterId, itemId, 1);
    }

    public bool MoveItem(int characterId, int itemId, int newSlot)
    {
        if (!_characterInventories.TryGetValue(characterId, out var inventory))
        {
            return false;
        }

        var item = inventory.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return false;
        }

        if (newSlot < 0 || newSlot >= MaxInventorySlots)
        {
            return false;
        }

        _logger.LogInformation($"Character {characterId} moved item {itemId} to slot {newSlot}");
        return true;
    }

    public bool HasSpace(int characterId)
    {
        if (!_characterInventories.TryGetValue(characterId, out var inventory))
        {
            return true;
        }

        return inventory.Count < MaxInventorySlots && GetInventoryWeight(characterId) < MaxInventoryWeight;
    }

    public int GetInventoryWeight(int characterId)
    {
        if (!_characterInventories.TryGetValue(characterId, out var inventory))
        {
            return 0;
        }

        return inventory.Sum(i => i.Weight * i.Quantity);
    }

    public int GetMaxInventoryWeight()
    {
        return MaxInventoryWeight;
    }
}

