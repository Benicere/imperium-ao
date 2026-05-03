using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Skills;

public class SkillSystem : ISkillSystem
{
    private readonly Dictionary<int, Dictionary<SkillType, Skill>> _characterSkills = new();
    private readonly ILogger<SkillSystem> _logger;

    public SkillSystem(ILogger<SkillSystem> logger)
    {
        _logger = logger;
    }

    public bool LearnSkill(int characterId, SkillType skillType)
    {
        if (!_characterSkills.ContainsKey(characterId))
        {
            _characterSkills[characterId] = new();
        }

        if (_characterSkills[characterId].ContainsKey(skillType))
        {
            _logger.LogWarning($"Character {characterId} already knows skill {skillType}");
            return false;
        }

        var skill = new Skill
        {
            Name = skillType.ToString(),
            Category = GetCategoryForSkill(skillType),
            MinLevel = 1,
            MaxLevel = 100,
            BaseExp = 100,
            CurrentLevel = 1,
            CurrentExp = 0,
            Type = skillType
        };

        _characterSkills[characterId][skillType] = skill;
        _logger.LogInformation($"Character {characterId} learned {skillType}");
        return true;
    }

    public bool IncreaseSkill(int characterId, SkillType skillType, int amount = 1)
    {
        if (!_characterSkills.TryGetValue(characterId, out var skills))
        {
            _logger.LogWarning($"Character {characterId} has no skills");
            return false;
        }

        if (!skills.TryGetValue(skillType, out var skill))
        {
            _logger.LogWarning($"Character {characterId} doesn't have skill {skillType}");
            return false;
        }

        skill.CurrentExp += amount;
        var expNeeded = skill.BaseExp * skill.CurrentLevel;

        if (skill.CurrentExp >= expNeeded && skill.CurrentLevel < skill.MaxLevel)
        {
            skill.CurrentLevel++;
            skill.CurrentExp = 0;
            _logger.LogInformation($"Character {characterId} leveled up {skillType} to {skill.CurrentLevel}");
        }

        return true;
    }

    public int GetSkillLevel(int characterId, SkillType skillType)
    {
        if (!_characterSkills.TryGetValue(characterId, out var skills))
        {
            return 0;
        }

        if (!skills.TryGetValue(skillType, out var skill))
        {
            return 0;
        }

        return skill.CurrentLevel;
    }

    public List<Skill> GetCharacterSkills(int characterId)
    {
        if (!_characterSkills.TryGetValue(characterId, out var skills))
        {
            return new();
        }

        return skills.Values.ToList();
    }

    public double GetSkillBonus(int characterId, SkillType skillType)
    {
        var level = GetSkillLevel(characterId, skillType);
        return level * 0.01;
    }

    public void ResetSkill(int characterId, SkillType skillType)
    {
        if (!_characterSkills.TryGetValue(characterId, out var skills))
        {
            return;
        }

        if (skills.TryGetValue(skillType, out var skill))
        {
            skill.CurrentLevel = 1;
            skill.CurrentExp = 0;
            _logger.LogInformation($"Character {characterId} skill {skillType} has been reset");
        }
    }

    private SkillCategory GetCategoryForSkill(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Sword or SkillType.Axe or SkillType.Blunt or SkillType.Dagger or SkillType.Bow => SkillCategory.Combat,
            SkillType.Staff or SkillType.Healing or SkillType.Fire or SkillType.Ice or SkillType.Lightning => SkillCategory.Magic,
            SkillType.Mining or SkillType.Woodcutting or SkillType.Tailoring or SkillType.Blacksmithing or SkillType.Alchemy => SkillCategory.Crafting,
            SkillType.Fishing or SkillType.Cooking => SkillCategory.Survival,
            SkillType.Persuasion or SkillType.Deception or SkillType.Intimidation => SkillCategory.Social,
            _ => SkillCategory.Combat
        };
    }
}


