using System;
namespace ImperiumAO.Server.Systems.Combat;

public class CombatStats
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public int Stamina { get; set; }
    public int MaxStamina { get; set; }

    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }

    public int PhysicalDamage { get; set; }
    public int MagicalDamage { get; set; }
    public int PhysicalDefense { get; set; }
    public int MagicalDefense { get; set; }

    public float DamageMultiplier { get; set; } = 1.0f;
    public float DefenseMultiplier { get; set; } = 1.0f;

    public int Level { get; set; } = 1;
    public int Experience { get; set; }
}

