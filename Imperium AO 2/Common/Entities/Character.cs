using System;

namespace ImperiumAO.Common.Entities;

public abstract class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public int Stamina { get; set; }
    public int MaxStamina { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Map { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public virtual bool IsAlive() => Health > 0;
}
