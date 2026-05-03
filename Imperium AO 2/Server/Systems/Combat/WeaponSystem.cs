using System;
namespace ImperiumAO.Server.Systems.Combat;

public class Weapon
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public WeaponType Type { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public int Weight { get; set; }
    public int RequiredLevel { get; set; }
    public int RequiredStrength { get; set; }
    public float AttackSpeed { get; set; }
    public int Durability { get; set; }
    public int MaxDurability { get; set; }
}

public enum WeaponType
{
    Sword,
    Axe,
    Hammer,
    Spear,
    Bow,
    Staff,
    Dagger,
    Mace
}

public interface IWeaponSystem
{
    int CalculateWeaponDamage(Weapon weapon);
    void DamageWeapon(Weapon weapon, int damageAmount = 1);
    bool IsWeaponBroken(Weapon weapon);
    void RepairWeapon(Weapon weapon, int repairAmount);
}

public class WeaponSystem : IWeaponSystem
{
    private readonly Random _random = new();

    public int CalculateWeaponDamage(Weapon weapon)
    {
        var variance = weapon.MaxDamage - weapon.MinDamage;
        var damage = weapon.MinDamage + _random.Next(variance + 1);

        var durabilityFactor = (float)weapon.Durability / weapon.MaxDurability;
        damage = (int)(damage * (0.5f + durabilityFactor * 0.5f));

        return damage;
    }

    public void DamageWeapon(Weapon weapon, int damageAmount = 1)
    {
        weapon.Durability = Math.Max(0, weapon.Durability - damageAmount);
    }

    public bool IsWeaponBroken(Weapon weapon)
    {
        return weapon.Durability <= 0;
    }

    public void RepairWeapon(Weapon weapon, int repairAmount)
    {
        weapon.Durability = Math.Min(weapon.MaxDurability, weapon.Durability + repairAmount);
    }
}

