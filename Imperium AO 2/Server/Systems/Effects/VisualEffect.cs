using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Effects;

public enum EffectAnimationType
{
    Projectile,
    Explosion,
    Heal,
    Buff,
    Debuff,
    Death,
    Teleport,
    Slash,
    Magic,
    Freeze
}

public class VisualEffect
{
    public int Id { get; set; }
    public EffectAnimationType Type { get; set; }
    public int SourceX { get; set; }
    public int SourceY { get; set; }
    public int SourceZ { get; set; }
    public int TargetX { get; set; }
    public int TargetY { get; set; }
    public int TargetZ { get; set; }
    public int DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLooping { get; set; }
    public bool IsExpired => (DateTime.UtcNow - CreatedAt).TotalMilliseconds > DurationMs;
    public int Intensity { get; set; }
    public string Color { get; set; } = "#FFFFFF";
}

public class Particle
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public float VelocityZ { get; set; }
    public int DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; } = "default";
    public bool IsExpired => (DateTime.UtcNow - CreatedAt).TotalMilliseconds > DurationMs;

    public void Update()
    {
        X += (int)VelocityX;
        Y += (int)VelocityY;
        Z += (int)VelocityZ;
    }
}

