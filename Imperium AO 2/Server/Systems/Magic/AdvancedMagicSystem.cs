using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Systems.Magic;

public class AdvancedMagicSystem : IAdvancedMagicSystem
{
    private readonly Dictionary<int, List<AdvancedSpell>> _playerSpells = new();
    private readonly Dictionary<int, List<SpellSchoolProgression>> _schoolProgressions = new();
    private readonly Dictionary<int, AdvancedSpell> _spellDatabase = new();
    private readonly ILogger<AdvancedMagicSystem> _logger;

    public AdvancedMagicSystem(ILogger<AdvancedMagicSystem> logger)
    {
        _logger = logger;
        InitializeSpellDatabase();
    }

    private void InitializeSpellDatabase()
    {
        _spellDatabase[1] = new()
        {
            Id = 1,
            Name = "Fireball",
            School = SpellSchool.Pyromancy,
            CastingType = SpellCastingType.Instant,
            ManaCost = 50,
            CastTime = 1,
            Cooldown = 5,
            RequiredLevel = 10,
            MaxRange = 30,
            Damage = 75,
            CriticalChance = 0.15f,
            CriticalMultiplier = 1.5f,
            IsAoeSpell = true,
            AoeRadius = 10
        };

        _spellDatabase[2] = new()
        {
            Id = 2,
            Name = "Frost Nova",
            School = SpellSchool.Cryomancy,
            CastingType = SpellCastingType.Instant,
            ManaCost = 40,
            CastTime = 0,
            Cooldown = 8,
            RequiredLevel = 8,
            MaxRange = 25,
            Damage = 40,
            EffectType = Combat.EffectType.Slow,
            EffectDuration = 5,
            IsAoeSpell = true,
            AoeRadius = 15
        };

        _spellDatabase[3] = new()
        {
            Id = 3,
            Name = "Healing Touch",
            School = SpellSchool.Restoration,
            CastingType = SpellCastingType.Instant,
            ManaCost = 35,
            CastTime = 2,
            Cooldown = 3,
            RequiredLevel = 5,
            MaxRange = 40,
            Healing = 100
        };
    }

    public void LearnSpell(int playerId, AdvancedSpell spell)
    {
        if (!_playerSpells.ContainsKey(playerId))
        {
            _playerSpells[playerId] = new();
        }

        if (!_playerSpells[playerId].Any(s => s.Id == spell.Id))
        {
            _playerSpells[playerId].Add(spell);
            _logger.LogInformation($"Player {playerId} learned spell {spell.Name}");
        }
    }

    public void ForgetSpell(int playerId, int spellId)
    {
        if (_playerSpells.TryGetValue(playerId, out var spells))
        {
            spells.RemoveAll(s => s.Id == spellId);
            _logger.LogInformation($"Player {playerId} forgot spell {spellId}");
        }
    }

    public AdvancedSpell? GetSpell(int spellId)
    {
        _spellDatabase.TryGetValue(spellId, out var spell);
        return spell;
    }

    public List<AdvancedSpell> GetPlayerSpells(int playerId)
    {
        if (_playerSpells.TryGetValue(playerId, out var spells))
        {
            return spells;
        }
        return new();
    }

    public List<AdvancedSpell> GetSpellsBySchool(int playerId, SpellSchool school)
    {
        return GetPlayerSpells(playerId).Where(s => s.School == school).ToList();
    }

    public bool CanCastSpell(int playerId, int spellId)
    {
        var spell = GetSpell(spellId);
        if (spell == null) return false;

        var playerSpells = GetPlayerSpells(playerId);
        var hasSpell = playerSpells.Any(s => s.Id == spellId);

        return hasSpell && !spell.IsOnCooldown;
    }

    public void CastSpell(int playerId, int spellId, int targetId)
    {
        if (!CanCastSpell(playerId, spellId))
        {
            _logger.LogWarning($"Player {playerId} cannot cast spell {spellId}");
            return;
        }

        var spell = GetSpell(spellId);
        if (spell != null)
        {
            spell.LastCastAt = DateTime.UtcNow;
            GainSchoolExperience(playerId, spell.School, 10);
            _logger.LogInformation($"Player {playerId} cast {spell.Name} on target {targetId}");
        }
    }

    public void GainSchoolExperience(int playerId, SpellSchool school, int amount)
    {
        if (!_schoolProgressions.ContainsKey(playerId))
        {
            _schoolProgressions[playerId] = new();
        }

        var progression = _schoolProgressions[playerId].FirstOrDefault(p => p.School == school);
        if (progression == null)
        {
            progression = new() { PlayerId = playerId, School = school, Level = 1 };
            _schoolProgressions[playerId].Add(progression);
        }

        var oldLevel = progression.Level;
        progression.GainExperience(amount);

        if (progression.Level > oldLevel)
        {
            _logger.LogInformation($"Player {playerId} {school} leveled up to {progression.Level}");
        }
    }

    public SpellSchoolProgression? GetSchoolProgression(int playerId, SpellSchool school)
    {
        if (_schoolProgressions.TryGetValue(playerId, out var progressions))
        {
            return progressions.FirstOrDefault(p => p.School == school);
        }
        return null;
    }

    public List<AdvancedSpell> GetAoeSpells()
    {
        return _spellDatabase.Values.Where(s => s.IsAoeSpell).ToList();
    }
}


