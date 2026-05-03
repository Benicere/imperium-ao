using System;
using System.Collections.Generic;

using ImperiumAO.Server.Systems.Combat;

namespace ImperiumAO.Server.Systems.Magic;

public enum SpellSchool
{
    Pyromancy,
    Cryomancy,
    Electromancy,
    Necromancy,
    Restoration,
    Transmutation,
    Divination
}

public enum SpellCastingType
{
    Instant,
    Channeled,
    Ritual,
    Passive
}

public class AdvancedSpell
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SpellSchool School { get; set; }
    public SpellCastingType CastingType { get; set; }
    public int ManaCost { get; set; }
    public int CastTime { get; set; }
    public int Cooldown { get; set; }
    public int RequiredLevel { get; set; }
    public int MaxRange { get; set; }
    public int Damage { get; set; }
    public int Healing { get; set; }
    public EffectType EffectType { get; set; }
    public int EffectDuration { get; set; }
    public float CriticalChance { get; set; }
    public float CriticalMultiplier { get; set; }
    public DateTime LastCastAt { get; set; }
    public bool IsAoeSpell { get; set; }
    public int AoeRadius { get; set; }

    public bool IsOnCooldown => (DateTime.UtcNow - LastCastAt).TotalSeconds < Cooldown;
    public int SecondsUntilCastable => IsOnCooldown ? (int)(Cooldown - (DateTime.UtcNow - LastCastAt).TotalSeconds) : 0;
}

public class SpellSchoolProgression
{
    public int PlayerId { get; set; }
    public SpellSchool School { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public List<AdvancedSpell> LearnedSpells { get; set; } = new();

    public int ExperienceForNextLevel => 100 * (Level + 1);

    public void GainExperience(int amount)
    {
        Experience += amount;
        while (Experience >= ExperienceForNextLevel)
        {
            Experience -= ExperienceForNextLevel;
            Level++;
        }
    }
}

