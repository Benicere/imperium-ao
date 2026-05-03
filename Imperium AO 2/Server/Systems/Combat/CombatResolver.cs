using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Combat;

public interface ICombatResolver
{
    void StartCombat(int attackerId, int defenderId);
    void EndCombat(int characterId);
    bool IsInCombat(int characterId);
    CombatSession? GetCombatSession(int characterId);
    void UpdateCombatEffects(int characterId);
}

public class CombatSession
{
    public int AttackerId { get; set; }
    public int DefenderId { get; set; }
    public DateTime StartTime { get; set; }
    public CombatStats AttackerStats { get; set; } = new();
    public CombatStats DefenderStats { get; set; } = new();
    public int RoundCount { get; set; }
    public DateTime LastActionTime { get; set; }
    public bool IsActive { get; set; }
    public List<CombatEffect> ActiveEffects { get; set; } = new();
}

public class CombatResolver : ICombatResolver
{
    private readonly Dictionary<int, CombatSession> _activeCombats = new();
    private readonly IEffectSystem _effectSystem;
    private readonly ILogger<CombatResolver> _logger;

    public CombatResolver(IEffectSystem effectSystem, ILogger<CombatResolver> logger)
    {
        _effectSystem = effectSystem;
        _logger = logger;
    }

    public void StartCombat(int attackerId, int defenderId)
    {
        if (_activeCombats.ContainsKey(attackerId))
        {
            _logger.LogWarning($"Character {attackerId} is already in combat");
            return;
        }

        var session = new CombatSession
        {
            AttackerId = attackerId,
            DefenderId = defenderId,
            StartTime = DateTime.UtcNow,
            IsActive = true,
            LastActionTime = DateTime.UtcNow
        };

        _activeCombats[attackerId] = session;
        _activeCombats[defenderId] = session;

        _logger.LogInformation($"Combat started between {attackerId} and {defenderId}");
    }

    public void EndCombat(int characterId)
    {
        if (_activeCombats.TryGetValue(characterId, out var session))
        {
            session.IsActive = false;
            _activeCombats.Remove(characterId);
            _activeCombats.Remove(session.AttackerId == characterId ? session.DefenderId : session.AttackerId);

            _logger.LogInformation($"Combat ended for character {characterId}");
        }
    }

    public bool IsInCombat(int characterId)
    {
        return _activeCombats.ContainsKey(characterId);
    }

    public CombatSession? GetCombatSession(int characterId)
    {
        _activeCombats.TryGetValue(characterId, out var session);
        return session;
    }

    public void UpdateCombatEffects(int characterId)
    {
        _effectSystem.UpdateEffects(characterId);
        if (_activeCombats.TryGetValue(characterId, out var session))
        {
            session.ActiveEffects = _effectSystem.GetActiveEffects(characterId);
        }
    }
}

