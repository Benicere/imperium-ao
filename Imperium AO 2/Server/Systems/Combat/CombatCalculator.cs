using System;
namespace ImperiumAO.Server.Systems.Combat;

public class CombatCalculator : ICombatCalculator
{
    private readonly IDamageCalculator _damageCalculator;
    private readonly Random _random = new();

    public CombatCalculator(IDamageCalculator damageCalculator)
    {
        _damageCalculator = damageCalculator;
    }

    public CombatResult ResolveCombat(CombatStats attacker, CombatStats defender, int weaponDamage = 0)
    {
        var result = new CombatResult();

        var dodgeChance = _damageCalculator.CalculateDodgeChance(defender);
        if (_random.NextSingle() < dodgeChance)
        {
            result.IsDodged = true;
            result.ActualDamage = 0;
            result.ResultType = CombatResultType.Dodge;
            return result;
        }

        var baseDamage = _damageCalculator.CalculatePhysicalDamage(attacker, defender) + weaponDamage;
        result.DamageDealt = baseDamage;

        var critChance = _damageCalculator.CalculateCriticalChance(attacker);
        if (_random.NextSingle() < critChance)
        {
            baseDamage = (int)(baseDamage * 1.5f);
            result.IsCritical = true;
        }

        result.ActualDamage = baseDamage;
        result.ResultType = result.IsCritical ? CombatResultType.CriticalHit : CombatResultType.Hit;

        ApplyDamage(defender, baseDamage);

        return result;
    }

    public CombatResult ResolveSpellCombat(CombatStats attacker, CombatStats defender, Spell spell)
    {
        var result = new CombatResult();

        if (attacker.Mana < spell.ManaCost)
        {
            result.ResultType = CombatResultType.Miss;
            result.ActualDamage = 0;
            return result;
        }

        attacker.Mana -= spell.ManaCost;

        var dodgeChance = _damageCalculator.CalculateDodgeChance(defender) * 0.5f;
        if (_random.NextSingle() < dodgeChance)
        {
            result.IsDodged = true;
            result.ActualDamage = 0;
            result.ResultType = CombatResultType.Dodge;
            return result;
        }

        var spellDamage = spell.BaseDamage + attacker.Intelligence / 3;
        var finalDamage = (int)(spellDamage * attacker.DamageMultiplier);

        var critChance = _damageCalculator.CalculateCriticalChance(attacker) * 0.6f;
        if (_random.NextSingle() < critChance)
        {
            finalDamage = (int)(finalDamage * 1.75f);
            result.IsCritical = true;
        }

        result.DamageDealt = finalDamage;
        result.ActualDamage = finalDamage;
        result.ResultType = result.IsCritical ? CombatResultType.CriticalHit : CombatResultType.Hit;

        ApplyDamage(defender, finalDamage);

        return result;
    }

    public void ApplyDamage(CombatStats target, int damage)
    {
        target.Health = Math.Max(0, target.Health - damage);
    }

    public bool IsAlive(CombatStats stats)
    {
        return stats.Health > 0;
    }
}

