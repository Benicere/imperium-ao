using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Skills;

public class ProfessionSystem : IProfessionSystem
{
    private readonly Dictionary<int, List<Profession>> _playerProfessions = new();
    private readonly ILogger<ProfessionSystem> _logger;
    private const int MaxProfessions = 3;
    private const int MaxPrimaryProfessions = 2;

    public ProfessionSystem(ILogger<ProfessionSystem> logger)
    {
        _logger = logger;
    }

    public void LearnProfession(int playerId, ProfessionType type)
    {
        if (!_playerProfessions.ContainsKey(playerId))
        {
            _playerProfessions[playerId] = new();
        }

        var professions = _playerProfessions[playerId];

        if (professions.Any(p => p.Type == type))
        {
            _logger.LogWarning($"Player {playerId} already knows profession {type}");
            return;
        }

        if (professions.Count >= MaxProfessions)
        {
            _logger.LogWarning($"Player {playerId} has reached max professions");
            return;
        }

        var isPrimary = professions.Count(p => p.IsPrimary) < MaxPrimaryProfessions;

        var profession = new Profession
        {
            PlayerId = playerId,
            Type = type,
            Level = 1,
            Experience = 0,
            SkillPoints = 0,
            IsPrimary = isPrimary,
            LearnedAt = DateTime.UtcNow
        };

        professions.Add(profession);
        _logger.LogInformation($"Player {playerId} learned profession {type} (Primary: {isPrimary})");
    }

    public void GainProfessionExp(int playerId, ProfessionType type, int amount)
    {
        if (!_playerProfessions.TryGetValue(playerId, out var professions))
        {
            _logger.LogWarning($"Player {playerId} has no professions");
            return;
        }

        var profession = professions.FirstOrDefault(p => p.Type == type);
        if (profession != null)
        {
            var oldLevel = profession.Level;
            profession.GainExperience(amount);

            if (profession.Level > oldLevel)
            {
                _logger.LogInformation($"Player {playerId} {type} leveled up to {profession.Level}");
            }
        }
    }

    public Profession? GetProfession(int playerId, ProfessionType type)
    {
        if (_playerProfessions.TryGetValue(playerId, out var professions))
        {
            return professions.FirstOrDefault(p => p.Type == type);
        }
        return null;
    }

    public List<Profession> GetPlayerProfessions(int playerId)
    {
        if (_playerProfessions.TryGetValue(playerId, out var professions))
        {
            return professions;
        }
        return new();
    }

    public bool CanCraft(int playerId, ProfessionType type)
    {
        var profession = GetProfession(playerId, type);
        return profession != null && profession.Level >= 1;
    }

    public void RemoveProfession(int playerId, ProfessionType type)
    {
        if (_playerProfessions.TryGetValue(playerId, out var professions))
        {
            professions.RemoveAll(p => p.Type == type);
            _logger.LogInformation($"Player {playerId} forgot profession {type}");
        }
    }
}

