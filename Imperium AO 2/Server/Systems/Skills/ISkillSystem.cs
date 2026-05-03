using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Skills;

public interface ISkillSystem
{
    bool LearnSkill(int characterId, SkillType skillType);
    bool IncreaseSkill(int characterId, SkillType skillType, int amount = 1);
    int GetSkillLevel(int characterId, SkillType skillType);
    List<Skill> GetCharacterSkills(int characterId);
    double GetSkillBonus(int characterId, SkillType skillType);
    void ResetSkill(int characterId, SkillType skillType);
}

