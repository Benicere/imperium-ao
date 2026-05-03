using System;
namespace ImperiumAO.Server.Systems.Skills;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public SkillCategory Category { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public int BaseExp { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentExp { get; set; }
    public SkillType Type { get; set; }
}

public enum SkillCategory
{
    Combat,
    Magic,
    Crafting,
    Survival,
    Social
}

public enum SkillType
{
    Sword,
    Axe,
    Blunt,
    Dagger,
    Bow,
    Staff,
    Healing,
    Fire,
    Ice,
    Lightning,
    Mining,
    Woodcutting,
    Tailoring,
    Blacksmithing,
    Alchemy,
    Fishing,
    Cooking,
    Persuasion,
    Deception,
    Intimidation
}

