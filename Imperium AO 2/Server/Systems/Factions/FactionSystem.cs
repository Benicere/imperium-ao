using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Factions;

public class FactionSystem : IFactionSystem
{
    private readonly Dictionary<int, CharacterFaction> _characterFactions = new();
    private readonly ILogger<FactionSystem> _logger;

    public FactionSystem(ILogger<FactionSystem> logger)
    {
        _logger = logger;
    }

    public void SetCharacterFaction(int characterId, Faction faction)
    {
        _characterFactions[characterId] = new CharacterFaction
        {
            CharacterId = characterId,
            Faction = faction,
            Reputation = 0
        };

        _logger.LogInformation($"Character {characterId} faction set to {faction}");
    }

    public Faction GetCharacterFaction(int characterId)
    {
        if (!_characterFactions.TryGetValue(characterId, out var charFaction))
        {
            return Faction.Neutral;
        }

        return charFaction.Faction;
    }

    public void AddReputation(int characterId, int amount)
    {
        if (!_characterFactions.TryGetValue(characterId, out var charFaction))
        {
            return;
        }

        charFaction.Reputation += amount;
        _logger.LogInformation($"Character {characterId} gained {amount} reputation (Total: {charFaction.Reputation})");
    }

    public int GetReputation(int characterId)
    {
        if (!_characterFactions.TryGetValue(characterId, out var charFaction))
        {
            return 0;
        }

        return charFaction.Reputation;
    }

    public FactionRank GetFactionRank(int characterId)
    {
        var reputation = GetReputation(characterId);

        return reputation switch
        {
            < 0 => FactionRank.Hated,
            < 500 => FactionRank.Unfriendly,
            < 1500 => FactionRank.Neutral,
            < 3000 => FactionRank.Friendly,
            < 5000 => FactionRank.Honored,
            _ => FactionRank.Exalted
        };
    }

    public bool CanInteractWithFaction(int characterId, Faction requiredFaction)
    {
        var charFaction = GetCharacterFaction(characterId);
        var rank = GetFactionRank(characterId);

        if (charFaction != requiredFaction)
        {
            return false;
        }

        return rank >= FactionRank.Friendly;
    }

    public List<int> GetCharactersByFaction(Faction faction)
    {
        return _characterFactions
            .Where(kv => kv.Value.Faction == faction)
            .Select(kv => kv.Key)
            .ToList();
    }

    public bool IsEnemy(int characterId1, int characterId2)
    {
        var faction1 = GetCharacterFaction(characterId1);
        var faction2 = GetCharacterFaction(characterId2);

        if (faction1 == Faction.Neutral || faction2 == Faction.Neutral)
        {
            return false;
        }

        return faction1 != faction2;
    }
}

public class CharacterFaction
{
    public int CharacterId { get; set; }
    public Faction Faction { get; set; }
    public int Reputation { get; set; }
}

public enum FactionRank
{
    Hated = 0,
    Unfriendly = 1,
    Neutral = 2,
    Friendly = 3,
    Honored = 4,
    Exalted = 5
}

