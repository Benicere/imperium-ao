using System;
namespace ImperiumAO.Server.Systems.Combat;

public interface ICombatCalculator
{
    CombatResult ResolveCombat(CombatStats attacker, CombatStats defender, int weaponDamage = 0);
    CombatResult ResolveSpellCombat(CombatStats attacker, CombatStats defender, Spell spell);
    void ApplyDamage(CombatStats target, int damage);
    bool IsAlive(CombatStats stats);
}

public class CombatResult
{
    public int DamageDealt { get; set; }
    public int ActualDamage { get; set; }
    public bool IsCritical { get; set; }
    public bool IsDodged { get; set; }
    public bool IsBlocked { get; set; }
    public CombatResultType ResultType { get; set; }
}

public enum CombatResultType
{
    Hit,
    Dodge,
    Block,
    CriticalHit,
    Miss
}

public class Spell
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int ManaCost { get; set; }
    public int BaseDamage { get; set; }
    public DamageType DamageType { get; set; }
    public float CastTime { get; set; }
    public int Range { get; set; }
}

