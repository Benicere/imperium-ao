using ImperiumAO.Common.Entities;
using Microsoft.Extensions.Logging;
using System;

namespace ImperiumAO.Server;

public class SkillSystem : ISkillSystem
{
    private readonly ILogger<SkillSystem> _logger;

    public SkillSystem(ILogger<SkillSystem> logger)
    {
        _logger = logger;
    }

    public void UseSkill(Character character, int skillId, Character? target = null)
    {
        if (character is not Player player)
        {
            _logger.LogDebug("Non-player cannot use skills");
            return;
        }

        var skillLevel = GetSkillLevel(player, skillId);
        if (skillLevel <= 0)
        {
            _logger.LogDebug("{Player} does not have skill {SkillId}", player.Name, skillId);
            return;
        }

        _logger.LogDebug("{Player} used skill {SkillId} at level {Level}",
            player.Name, skillId, skillLevel);
    }

    public int GetSkillLevel(Character character, int skillId)
    {
        return 0;
    }
}

