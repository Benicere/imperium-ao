using System;
using System.Linq;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Combat;

public class Armor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ArmorSlot Slot { get; set; }
    public int PhysicalDefense { get; set; }
    public int MagicalDefense { get; set; }
    public int Weight { get; set; }
    public int RequiredLevel { get; set; }
    public int Durability { get; set; }
    public int MaxDurability { get; set; }
}

public enum ArmorSlot
{
    Head,
    Chest,
    Legs,
    Feet,
    Hands,
    Back
}

public interface IArmorSystem
{
    int CalculateTotalPhysicalDefense(Dictionary<ArmorSlot, Armor> equippedArmor);
    int CalculateTotalMagicalDefense(Dictionary<ArmorSlot, Armor> equippedArmor);
    void DamageArmor(Armor armor, int damageAmount = 1);
    bool IsArmorBroken(Armor armor);
    void RepairArmor(Armor armor, int repairAmount);
    int CalculateWeightPenalty(Dictionary<ArmorSlot, Armor> equippedArmor);
}

public class ArmorSystem : IArmorSystem
{
    public int CalculateTotalPhysicalDefense(Dictionary<ArmorSlot, Armor> equippedArmor)
    {
        return equippedArmor.Values.Sum(a => a.PhysicalDefense);
    }

    public int CalculateTotalMagicalDefense(Dictionary<ArmorSlot, Armor> equippedArmor)
    {
        return equippedArmor.Values.Sum(a => a.MagicalDefense);
    }

    public void DamageArmor(Armor armor, int damageAmount = 1)
    {
        armor.Durability = Math.Max(0, armor.Durability - damageAmount);
    }

    public bool IsArmorBroken(Armor armor)
    {
        return armor.Durability <= 0;
    }

    public void RepairArmor(Armor armor, int repairAmount)
    {
        armor.Durability = Math.Min(armor.MaxDurability, armor.Durability + repairAmount);
    }

    public int CalculateWeightPenalty(Dictionary<ArmorSlot, Armor> equippedArmor)
    {
        var totalWeight = equippedArmor.Values.Sum(a => a.Weight);
        var basePenalty = totalWeight / 10;
        return Math.Max(0, basePenalty);
    }
}


