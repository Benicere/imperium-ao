using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Combat;

public class EffectSystem : IEffectSystem
{
    private readonly Dictionary<int, List<CombatEffect>> _characterEffects = new();
    private readonly Dictionary<int, List<CrowdControlEffect>> _crowdControls = new();
    private readonly ILogger<EffectSystem> _logger;
    private int _effectIdCounter = 1;

    public EffectSystem(ILogger<EffectSystem> logger)
    {
        _logger = logger;
    }

    public void ApplyEffect(int targetId, int sourceId, EffectType type, int damage, int duration)
    {
        if (!_characterEffects.ContainsKey(targetId))
        {
            _characterEffects[targetId] = new();
        }

        var effect = new CombatEffect
        {
            Id = _effectIdCounter++,
            TargetId = targetId,
            SourceId = sourceId,
            Type = type,
            Damage = damage,
            Duration = duration,
            AppliedAt = DateTime.UtcNow,
            IsActive = true,
            Effectiveness = 1.0f,
            TotalTicks = duration / 2
        };

        _characterEffects[targetId].Add(effect);
        _logger.LogInformation($"Applied {type} to character {targetId} for {duration}s, {damage} damage per tick");

        if (type is EffectType.Stun or EffectType.Slow or EffectType.Root or EffectType.Silence or EffectType.Fear)
        {
            ApplyCrowdControl(targetId, type, duration);
        }
    }

    public void ApplyCrowdControl(int targetId, EffectType type, int duration)
    {
        if (!_crowdControls.ContainsKey(targetId))
        {
            _crowdControls[targetId] = new();
        }

        var cc = new CrowdControlEffect
        {
            TargetId = targetId,
            Type = type,
            Duration = duration,
            AppliedAt = DateTime.UtcNow
        };

        cc.ApplyRestrictions();
        _crowdControls[targetId].Add(cc);
        _logger.LogInformation($"Applied crowd control {type} to character {targetId}");
    }

    public void RemoveEffect(int targetId, int effectId)
    {
        if (_characterEffects.TryGetValue(targetId, out var effects))
        {
            effects.RemoveAll(e => e.Id == effectId);
        }
    }

    public void ClearEffects(int targetId)
    {
        if (_characterEffects.TryGetValue(targetId, out var effects))
        {
            effects.Clear();
        }
        if (_crowdControls.TryGetValue(targetId, out var ccs))
        {
            ccs.Clear();
        }
    }

    public List<CombatEffect> GetActiveEffects(int characterId)
    {
        if (_characterEffects.TryGetValue(characterId, out var effects))
        {
            return effects.Where(e => e.IsActive && !e.IsExpired).ToList();
        }
        return new();
    }

    public int CalculateDoTDamage(int targetId)
    {
        var effects = GetActiveEffects(targetId);
        var totalDamage = 0;

        foreach (var effect in effects)
        {
            if (effect.Type is EffectType.DamageOverTime or EffectType.Bleed or EffectType.Poison)
            {
                if (effect.ShouldTick(2))
                {
                    totalDamage += (int)(effect.Damage * effect.Effectiveness);
                    effect.TickCount++;
                    _logger.LogInformation($"DoT tick on {targetId}: {effect.Damage} damage from {effect.Type}");
                }
            }
        }

        return totalDamage;
    }

    public bool HasCrowdControl(int characterId, EffectType type)
    {
        if (_crowdControls.TryGetValue(characterId, out var ccs))
        {
            return ccs.Any(cc => cc.Type == type && !cc.IsExpired);
        }
        return false;
    }

    public CrowdControlEffect? GetCrowdControl(int characterId, EffectType type)
    {
        if (_crowdControls.TryGetValue(characterId, out var ccs))
        {
            return ccs.FirstOrDefault(cc => cc.Type == type && !cc.IsExpired);
        }
        return null;
    }

    public void UpdateEffects(int characterId)
    {
        if (_characterEffects.TryGetValue(characterId, out var effects))
        {
            effects.RemoveAll(e => e.IsExpired);
            foreach (var effect in effects)
            {
                effect.IsActive = !effect.IsExpired;
            }
        }

        if (_crowdControls.TryGetValue(characterId, out var ccs))
        {
            ccs.RemoveAll(cc => cc.IsExpired);
        }
    }

    public bool CanPerformAction(int characterId, string actionType)
    {
        var ccs = _crowdControls.TryGetValue(characterId, out var list) ? list.Where(cc => !cc.IsExpired).ToList() : new();

        foreach (var cc in ccs)
        {
            return actionType switch
            {
                "move" => cc.CanMove,
                "cast" => cc.CanCast,
                "attack" => cc.CanAttack,
                _ => true
            };
        }

        return true;
    }
}

