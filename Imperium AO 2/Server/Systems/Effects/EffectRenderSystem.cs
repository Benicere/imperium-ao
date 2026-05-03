using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Systems.Effects;

public class EffectRenderSystem : IEffectRenderSystem
{
    private readonly Dictionary<int, VisualEffect> _effects = new();
    private readonly List<Particle> _particles = new();
    private readonly ILogger<EffectRenderSystem> _logger;
    private int _effectIdCounter = 1;
    private int _particleIdCounter = 1;

    public EffectRenderSystem(ILogger<EffectRenderSystem> logger)
    {
        _logger = logger;
    }

    public void PlayEffect(VisualEffect effect)
    {
        effect.Id = _effectIdCounter++;
        effect.CreatedAt = DateTime.UtcNow;
        _effects[effect.Id] = effect;
        _logger.LogInformation($"Effect {effect.Type} played at ({effect.SourceX}, {effect.SourceY})");
    }

    public void StopEffect(int effectId)
    {
        if (_effects.TryGetValue(effectId, out var effect))
        {
            _effects.Remove(effectId);
            _logger.LogInformation($"Effect {effectId} stopped");
        }
    }

    public VisualEffect? GetEffect(int effectId)
    {
        _effects.TryGetValue(effectId, out var effect);
        return effect;
    }

    public List<VisualEffect> GetEffectsInArea(int x, int y, int range)
    {
        return _effects.Values
            .Where(e => Math.Abs(e.SourceX - x) <= range && Math.Abs(e.SourceY - y) <= range && !e.IsExpired)
            .ToList();
    }

    public void EmitParticles(Particle particle, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var p = new Particle
            {
                Id = _particleIdCounter++,
                X = particle.X,
                Y = particle.Y,
                Z = particle.Z,
                VelocityX = particle.VelocityX * ((i % 2 == 0) ? 1 : -1),
                VelocityY = particle.VelocityY * ((i % 3 == 0) ? 1 : -1),
                VelocityZ = particle.VelocityZ,
                DurationMs = particle.DurationMs,
                CreatedAt = DateTime.UtcNow,
                Type = particle.Type
            };

            _particles.Add(p);
        }

        _logger.LogInformation($"Emitted {count} particles at ({particle.X}, {particle.Y})");
    }

    public List<Particle> GetParticles()
    {
        return _particles.Where(p => !p.IsExpired).ToList();
    }

    public void ClearExpiredEffects()
    {
        var expired = _effects.Where(kvp => kvp.Value.IsExpired).Select(kvp => kvp.Key).ToList();
        foreach (var effectId in expired)
        {
            _effects.Remove(effectId);
        }

        if (expired.Count > 0)
        {
            _logger.LogInformation($"Cleared {expired.Count} expired effects");
        }
    }

    public void ClearExpiredParticles()
    {
        var count = _particles.RemoveAll(p => p.IsExpired);
        if (count > 0)
        {
            _logger.LogInformation($"Cleared {count} expired particles");
        }
    }

    public void PlaySpellEffect(int spellId, int targetX, int targetY, int targetZ)
    {
        var effect = new VisualEffect
        {
            Type = EffectAnimationType.Magic,
            TargetX = targetX,
            TargetY = targetY,
            TargetZ = targetZ,
            DurationMs = 1000,
            Intensity = 100
        };

        PlayEffect(effect);
        _logger.LogInformation($"Spell {spellId} effect played at ({targetX}, {targetY}, {targetZ})");
    }

    public void PlayDamageEffect(int targetX, int targetY, int damage)
    {
        var effect = new VisualEffect
        {
            Type = EffectAnimationType.Slash,
            TargetX = targetX,
            TargetY = targetY,
            DurationMs = 500,
            Intensity = Math.Min(100, damage / 10)
        };

        PlayEffect(effect);
    }

    public void PlayHealEffect(int targetX, int targetY, int healAmount)
    {
        var effect = new VisualEffect
        {
            Type = EffectAnimationType.Heal,
            TargetX = targetX,
            TargetY = targetY,
            DurationMs = 800,
            Color = "#00FF00",
            Intensity = Math.Min(100, healAmount / 10)
        };

        PlayEffect(effect);
    }
}


