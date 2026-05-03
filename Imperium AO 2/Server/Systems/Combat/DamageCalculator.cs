using System;
namespace ImperiumAO.Server.Systems.Combat;

public class DamageCalculator : IDamageCalculator
{
    private readonly Random _random = new();

    public int CalculatePhysicalDamage(CombatStats attacker, CombatStats defender)
    {
        var baseDamage = attacker.PhysicalDamage + attacker.Strength / 2;
        var defenseReduction = defender.PhysicalDefense / 3;
        var armorReduction = (int)(baseDamage * (defender.DefenseMultiplier - 1));

        var damage = Math.Max(1, baseDamage - defenseReduction - armorReduction);

        var variance = (int)(damage * 0.15f);
        var randomVariance = _random.Next(-variance, variance + 1);

        return Math.Max(1, damage + randomVariance);
    }

    public int CalculateMagicalDamage(CombatStats attacker, CombatStats defender)
    {
        var baseDamage = attacker.MagicalDamage + attacker.Intelligence / 2;
        var spellResistance = defender.MagicalDefense / 3;

        var damage = Math.Max(1, baseDamage - spellResistance);

        var variance = (int)(damage * 0.1f);
        var randomVariance = _random.Next(-variance, variance + 1);

        return Math.Max(1, damage + randomVariance);
    }

    public int CalculateTotalDamage(CombatStats attacker, CombatStats defender, DamageType damageType)
    {
        var damage = damageType switch
        {
            DamageType.Physical => CalculatePhysicalDamage(attacker, defender),
            DamageType.Magical => CalculateMagicalDamage(attacker, defender),
            DamageType.Mixed => (CalculatePhysicalDamage(attacker, defender) + CalculateMagicalDamage(attacker, defender)) / 2,
            _ => 0
        };

        var multiplier = attacker.DamageMultiplier;
        return (int)(damage * multiplier);
    }

    public float CalculateCriticalChance(CombatStats attacker)
    {
        var baseChance = 0.05f;
        var agilityBonus = attacker.Agility / 1000f;
        return Math.Min(0.5f, baseChance + agilityBonus);
    }

    public float CalculateDodgeChance(CombatStats defender)
    {
        var baseChance = 0.05f;
        var agilityBonus = defender.Agility / 1500f;
        return Math.Min(0.4f, baseChance + agilityBonus);
    }
}

