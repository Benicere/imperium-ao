using System;
namespace ImperiumAO.Server.Systems.Combat;

public interface IDamageCalculator
{
    int CalculatePhysicalDamage(CombatStats attacker, CombatStats defender);
    int CalculateMagicalDamage(CombatStats attacker, CombatStats defender);
    int CalculateTotalDamage(CombatStats attacker, CombatStats defender, DamageType damageType);
    float CalculateCriticalChance(CombatStats attacker);
    float CalculateDodgeChance(CombatStats defender);
}

public enum DamageType
{
    Physical,
    Magical,
    Mixed
}

