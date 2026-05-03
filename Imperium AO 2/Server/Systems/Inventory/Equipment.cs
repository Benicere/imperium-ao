using System;
using System.Collections.Generic;
using System.Linq;
namespace ImperiumAO.Server.Systems.Inventory;

public enum EquipmentSlot
{
    Head,
    Chest,
    Legs,
    Feet,
    Hands,
    Back,
    LeftHand,
    RightHand,
    Neck,
    RingLeft,
    RingRight
}

public class EquippedItem
{
    public int ItemId { get; set; }
    public EquipmentSlot Slot { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ArmorValue { get; set; }
    public int DamageBonus { get; set; }
    public int HealthBonus { get; set; }
    public int ManaBonus { get; set; }
    public DateTime EquippedAt { get; set; }
    public int Durability { get; set; }
    public int MaxDurability { get; set; }

    public double DurabilityPercent => MaxDurability > 0 ? (double)Durability / MaxDurability * 100 : 0;
}

public class Equipment
{
    public int PlayerId { get; set; }
    private readonly Dictionary<EquipmentSlot, EquippedItem?> _slots = new();

    public Equipment()
    {
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            _slots[slot] = null;
        }
    }

    public bool Equip(EquippedItem item)
    {
        if (!_slots.ContainsKey(item.Slot))
            return false;

        _slots[item.Slot] = item;
        return true;
    }

    public EquippedItem? Unequip(EquipmentSlot slot)
    {
        var item = _slots[slot];
        _slots[slot] = null;
        return item;
    }

    public EquippedItem? GetEquipped(EquipmentSlot slot)
    {
        return _slots.TryGetValue(slot, out var item) ? item : null;
    }

    public List<EquippedItem> GetAllEquipped()
    {
        return _slots.Values.Where(i => i != null).Cast<EquippedItem>().ToList();
    }

    public int GetTotalArmorValue()
    {
        return GetAllEquipped().Sum(i => i.ArmorValue);
    }

    public int GetTotalDamageBonus()
    {
        return GetAllEquipped().Sum(i => i.DamageBonus);
    }

    public int GetTotalHealthBonus()
    {
        return GetAllEquipped().Sum(i => i.HealthBonus);
    }

    public int GetTotalManaBonus()
    {
        return GetAllEquipped().Sum(i => i.ManaBonus);
    }

    public void ReduceDurability(int amount)
    {
        var weapon = GetEquipped(EquipmentSlot.RightHand);
        if (weapon != null && weapon.Durability > 0)
        {
            weapon.Durability = Math.Max(0, weapon.Durability - amount);
        }
    }

    public bool IsSlotFull(EquipmentSlot slot)
    {
        return _slots[slot] != null;
    }
}

