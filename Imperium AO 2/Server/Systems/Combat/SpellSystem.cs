using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Combat;

public class SpellSystem : ISpellSystem
{
    private readonly SpellDatabase _spellDatabase;
    private readonly Dictionary<int, HashSet<int>> _characterSpells = new();
    private readonly ILogger<SpellSystem> _logger;

    public SpellSystem(SpellDatabase spellDatabase, ILogger<SpellSystem> logger)
    {
        _spellDatabase = spellDatabase;
        _logger = logger;
    }

    public Spell? GetSpell(int spellId)
    {
        return _spellDatabase.GetSpell(spellId);
    }

    public List<Spell> GetCharacterSpells(int characterId)
    {
        if (!_characterSpells.TryGetValue(characterId, out var spellIds))
        {
            return new();
        }

        return spellIds
            .Select(id => _spellDatabase.GetSpell(id))
            .Where(spell => spell != null)
            .Cast<Spell>()
            .ToList();
    }

    public bool LearnSpell(int characterId, int spellId)
    {
        var spell = _spellDatabase.GetSpell(spellId);
        if (spell == null)
        {
            _logger.LogWarning($"Spell {spellId} not found");
            return false;
        }

        if (!_characterSpells.ContainsKey(characterId))
        {
            _characterSpells[characterId] = new();
        }

        if (_characterSpells[characterId].Contains(spellId))
        {
            _logger.LogWarning($"Character {characterId} already knows spell {spellId}");
            return false;
        }

        _characterSpells[characterId].Add(spellId);
        _logger.LogInformation($"Character {characterId} learned spell {spellId}");
        return true;
    }

    public bool ForgetSpell(int characterId, int spellId)
    {
        if (!_characterSpells.TryGetValue(characterId, out var spellIds))
        {
            return false;
        }

        var removed = spellIds.Remove(spellId);
        if (removed)
        {
            _logger.LogInformation($"Character {characterId} forgot spell {spellId}");
        }

        return removed;
    }

    public bool CanCastSpell(int characterId, Spell spell)
    {
        if (!_characterSpells.TryGetValue(characterId, out var spellIds))
        {
            return false;
        }

        return spellIds.Contains(spell.Id);
    }

    public void CastSpell(int characterId, int targetId, Spell spell)
    {
        if (!CanCastSpell(characterId, spell))
        {
            _logger.LogWarning($"Character {characterId} cannot cast spell {spell.Id}");
            return;
        }

        _logger.LogInformation($"Character {characterId} cast {spell.Name} on {targetId}");
    }
}

