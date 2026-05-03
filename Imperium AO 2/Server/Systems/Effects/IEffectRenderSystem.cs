using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Effects;

public interface IEffectRenderSystem
{
    void PlayEffect(VisualEffect effect);
    void StopEffect(int effectId);
    VisualEffect? GetEffect(int effectId);
    List<VisualEffect> GetEffectsInArea(int x, int y, int range);
    void EmitParticles(Particle particle, int count);
    List<Particle> GetParticles();
    void ClearExpiredEffects();
    void ClearExpiredParticles();
    void PlaySpellEffect(int spellId, int targetX, int targetY, int targetZ);
    void PlayDamageEffect(int targetX, int targetY, int damage);
    void PlayHealEffect(int targetX, int targetY, int healAmount);
}

