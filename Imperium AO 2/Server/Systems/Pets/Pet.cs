using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Pets;

public enum PetType
{
    Wolf,
    Bear,
    Raven,
    Spider,
    Skeleton,
    Golem,
    Dragon
}

public class Pet
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public PetType Type { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public int Damage { get; set; }
    public int Defense { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public bool IsActive { get; set; }
    public DateTime SummonedAt { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsExpired => (DateTime.UtcNow - SummonedAt).TotalSeconds > DurationSeconds;

    public int ExperienceForNextLevel => 100 * (Level + 1);

    public void GainExperience(int amount)
    {
        Experience += amount;
        while (Experience >= ExperienceForNextLevel)
        {
            Experience -= ExperienceForNextLevel;
            Level++;
            Damage += 5;
            Health += 10;
            MaxHealth += 10;
        }
    }

    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }
}

