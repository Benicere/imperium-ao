using System;
namespace ImperiumAO.Server.Systems.Inventory;

public interface IEquipmentSystem
{
    bool EquipItem(int playerId, EquippedItem item);
    EquippedItem? UnequipItem(int playerId, EquipmentSlot slot);
    Equipment? GetPlayerEquipment(int playerId);
    int GetTotalArmor(int playerId);
    int GetTotalDamageBonus(int playerId);
    int GetTotalHealthBonus(int playerId);
    int GetTotalManaBonus(int playerId);
    void ReduceWeaponDurability(int playerId, int amount);
    bool CanEquipInSlot(int playerId, EquipmentSlot slot);
}

