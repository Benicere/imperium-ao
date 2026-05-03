using System;
using System.Linq;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Combat;

public class SpellDatabase
{
    private static readonly Dictionary<int, Spell> DefaultSpells = new()
    {
        { 1, new Spell { Id = 1, Name = "Fireball", ManaCost = 30, BaseDamage = 50, DamageType = DamageType.Magical, CastTime = 1.5f, Range = 8 } },
        { 2, new Spell { Id = 2, Name = "Frostbolt", ManaCost = 25, BaseDamage = 35, DamageType = DamageType.Magical, CastTime = 1.0f, Range = 10 } },
        { 3, new Spell { Id = 3, Name = "Lightning", ManaCost = 40, BaseDamage = 60, DamageType = DamageType.Magical, CastTime = 2.0f, Range = 12 } },
        { 4, new Spell { Id = 4, Name = "Heal", ManaCost = 20, BaseDamage = -30, DamageType = DamageType.Magical, CastTime = 1.5f, Range = 8 } },
        { 5, new Spell { Id = 5, Name = "Holy Light", ManaCost = 35, BaseDamage = 45, DamageType = DamageType.Magical, CastTime = 1.5f, Range = 8 } }
    };

    public Spell? GetSpell(int spellId)
    {
        DefaultSpells.TryGetValue(spellId, out var spell);
        return spell;
    }

    public List<Spell> GetAllSpells()
    {
        return DefaultSpells.Values.ToList();
    }
}


