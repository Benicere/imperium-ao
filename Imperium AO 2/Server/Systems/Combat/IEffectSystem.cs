using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Combat;

public interface IEffectSystem
{
    void ApplyEffect(int targetId, int sourceId, EffectType type, int damage, int duration);
    void RemoveEffect(int targetId, int effectId);
    void ClearEffects(int targetId);
    List<CombatEffect> GetActiveEffects(int characterId);
    int CalculateDoTDamage(int targetId);
    bool HasCrowdControl(int characterId, EffectType type);
    CrowdControlEffect? GetCrowdControl(int characterId, EffectType type);
    void UpdateEffects(int characterId);
    void ApplyCrowdControl(int targetId, EffectType type, int duration);
    bool CanPerformAction(int characterId, string actionType);
}

