using System;
namespace ImperiumAO.Server.Systems.Skills;

public enum ProfessionType
{
    Blacksmith,
    Alchemist,
    Tailor,
    Leatherworking,
    Woodcrafting,
    Mining,
    Herbalism
}

public class Profession
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public ProfessionType Type { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int SkillPoints { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime LearnedAt { get; set; }

    public int ExperienceForNextLevel => 100 * (Level + 1);

    public void GainExperience(int amount)
    {
        Experience += amount;
        while (Experience >= ExperienceForNextLevel)
        {
            Experience -= ExperienceForNextLevel;
            Level++;
            SkillPoints += 5;
        }
    }
}

