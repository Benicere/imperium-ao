using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Inventory;

public class EquipmentSystem : IEquipmentSystem
{
    private readonly Dictionary<int, Equipment> _playerEquipment = new();
    private readonly ILogger<EquipmentSystem> _logger;

    public EquipmentSystem(ILogger<EquipmentSystem> logger)
    {
        _logger = logger;
    }

    public bool EquipItem(int playerId, EquippedItem item)
    {
        if (!_playerEquipment.ContainsKey(playerId))
        {
            _playerEquipment[playerId] = new() { PlayerId = playerId };
        }

        var equipment = _playerEquipment[playerId];

        if (equipment.IsSlotFull(item.Slot))
        {
            _logger.LogWarning($"Slot {item.Slot} is already occupied for player {playerId}");
            return false;
        }

        if (!equipment.Equip(item))
        {
            _logger.LogWarning($"Failed to equip item in slot {item.Slot}");
            return false;
        }

        _logger.LogInformation($"Player {playerId} equipped {item.Name} in slot {item.Slot}");
        return true;
    }

    public EquippedItem? UnequipItem(int playerId, EquipmentSlot slot)
    {
        if (!_playerEquipment.TryGetValue(playerId, out var equipment))
        {
            return null;
        }

        var item = equipment.Unequip(slot);
        if (item != null)
        {
            _logger.LogInformation($"Player {playerId} unequipped {item.Name} from slot {slot}");
        }

        return item;
    }

    public Equipment? GetPlayerEquipment(int playerId)
    {
        if (!_playerEquipment.ContainsKey(playerId))
        {
            _playerEquipment[playerId] = new() { PlayerId = playerId };
        }

        return _playerEquipment[playerId];
    }

    public int GetTotalArmor(int playerId)
    {
        var equipment = GetPlayerEquipment(playerId);
        return equipment?.GetTotalArmorValue() ?? 0;
    }

    public int GetTotalDamageBonus(int playerId)
    {
        var equipment = GetPlayerEquipment(playerId);
        return equipment?.GetTotalDamageBonus() ?? 0;
    }

    public int GetTotalHealthBonus(int playerId)
    {
        var equipment = GetPlayerEquipment(playerId);
        return equipment?.GetTotalHealthBonus() ?? 0;
    }

    public int GetTotalManaBonus(int playerId)
    {
        var equipment = GetPlayerEquipment(playerId);
        return equipment?.GetTotalManaBonus() ?? 0;
    }

    public void ReduceWeaponDurability(int playerId, int amount)
    {
        var equipment = GetPlayerEquipment(playerId);
        equipment?.ReduceDurability(amount);
    }

    public bool CanEquipInSlot(int playerId, EquipmentSlot slot)
    {
        var equipment = GetPlayerEquipment(playerId);
        return equipment != null && !equipment.IsSlotFull(slot);
    }
}

