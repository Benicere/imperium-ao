using ImperiumAO.Common.Entities;
using System;

namespace ImperiumAO.Server;

public interface ISkillSystem
{
    void UseSkill(Character character, int skillId, Character? target = null);
    int GetSkillLevel(Character character, int skillId);
}

