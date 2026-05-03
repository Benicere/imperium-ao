using System;
namespace ImperiumAO.Server.Systems.Combat;

public enum EffectType
{
    DamageOverTime,
    Stun,
    Slow,
    Bleed,
    Poison,
    Curse,
    Root,
    Blind,
    Silence,
    Fear
}

public class CombatEffect
{
    public int Id { get; set; }
    public int TargetId { get; set; }
    public int SourceId { get; set; }
    public EffectType Type { get; set; }
    public int Duration { get; set; }
    public int Damage { get; set; }
    public float Effectiveness { get; set; }
    public DateTime AppliedAt { get; set; }
    public bool IsActive { get; set; }
    public int TickCount { get; set; }
    public int TotalTicks { get; set; }

    public bool IsExpired => DateTime.UtcNow >= AppliedAt.AddSeconds(Duration);

    public bool ShouldTick(int interval)
    {
        var elapsed = (DateTime.UtcNow - AppliedAt).TotalSeconds;
        var nextTick = (TickCount + 1) * interval;
        return elapsed >= nextTick && !IsExpired;
    }
}

public class CrowdControlEffect
{
    public int TargetId { get; set; }
    public EffectType Type { get; set; }
    public int Duration { get; set; }
    public DateTime AppliedAt { get; set; }
    public bool CanMove { get; set; }
    public bool CanCast { get; set; }
    public bool CanAttack { get; set; }

    public bool IsExpired => DateTime.UtcNow >= AppliedAt.AddSeconds(Duration);

    public void ApplyRestrictions()
    {
        switch (Type)
        {
            case EffectType.Stun:
                CanMove = CanCast = CanAttack = false;
                break;
            case EffectType.Root:
                CanMove = false;
                CanCast = CanAttack = true;
                break;
            case EffectType.Silence:
                CanCast = false;
                CanMove = CanAttack = true;
                break;
            case EffectType.Slow:
                CanMove = CanCast = CanAttack = true;
                break;
            case EffectType.Fear:
                CanAttack = false;
                CanMove = CanCast = true;
                break;
        }
    }
}

