using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Guilds;

public class GuildSystem : IGuildSystem
{
    private readonly Dictionary<int, Guild> _guilds = new();
    private readonly Dictionary<int, int> _characterToGuild = new();
    private int _nextGuildId = 1;
    private readonly ILogger<GuildSystem> _logger;

    public GuildSystem(ILogger<GuildSystem> logger)
    {
        _logger = logger;
    }

    public Guild? CreateGuild(string name, int leaderId)
    {
        if (_characterToGuild.ContainsKey(leaderId))
        {
            _logger.LogWarning($"Character {leaderId} is already in a guild");
            return null;
        }

        var guild = new Guild
        {
            Id = _nextGuildId++,
            Name = name,
            LeaderId = leaderId,
            Members = new()
            {
                new GuildMember { CharacterId = leaderId, Rank = GuildRank.Leader }
            }
        };

        _guilds[guild.Id] = guild;
        _characterToGuild[leaderId] = guild.Id;

        _logger.LogInformation($"Guild '{name}' created with leader {leaderId}");
        return guild;
    }

    public Guild? GetGuild(int guildId)
    {
        _guilds.TryGetValue(guildId, out var guild);
        return guild;
    }

    public Guild? GetGuildByName(string name)
    {
        return _guilds.Values.FirstOrDefault(g => g.Name == name);
    }

    public bool AddMember(int guildId, int characterId, string characterName)
    {
        if (!_guilds.TryGetValue(guildId, out var guild))
        {
            return false;
        }

        if (_characterToGuild.ContainsKey(characterId))
        {
            _logger.LogWarning($"Character {characterId} is already in a guild");
            return false;
        }

        if (guild.Members.Count >= guild.MaxMembers)
        {
            _logger.LogWarning($"Guild {guildId} is full");
            return false;
        }

        guild.Members.Add(new GuildMember { CharacterId = characterId, Name = characterName });
        _characterToGuild[characterId] = guildId;

        _logger.LogInformation($"Character {characterName} ({characterId}) joined guild {guild.Name}");
        return true;
    }

    public bool RemoveMember(int guildId, int characterId)
    {
        if (!_guilds.TryGetValue(guildId, out var guild))
        {
            return false;
        }

        var member = guild.Members.FirstOrDefault(m => m.CharacterId == characterId);
        if (member == null)
        {
            return false;
        }

        guild.Members.Remove(member);
        _characterToGuild.Remove(characterId);

        _logger.LogInformation($"Character {characterId} left guild {guild.Name}");
        return true;
    }

    public bool PromoteMember(int guildId, int characterId, GuildRank newRank)
    {
        if (!_guilds.TryGetValue(guildId, out var guild))
        {
            return false;
        }

        var member = guild.Members.FirstOrDefault(m => m.CharacterId == characterId);
        if (member == null)
        {
            return false;
        }

        member.Rank = newRank;
        _logger.LogInformation($"Character {characterId} promoted to {newRank} in guild {guild.Name}");
        return true;
    }

    public bool DisbandGuild(int guildId)
    {
        if (!_guilds.TryGetValue(guildId, out var guild))
        {
            return false;
        }

        foreach (var member in guild.Members)
        {
            _characterToGuild.Remove(member.CharacterId);
        }

        _guilds.Remove(guildId);
        _logger.LogInformation($"Guild {guild.Name} ({guildId}) has been disbanded");
        return true;
    }

    public List<Guild> GetAllGuilds()
    {
        return _guilds.Values.ToList();
    }

    public Guild? GetCharacterGuild(int characterId)
    {
        if (!_characterToGuild.TryGetValue(characterId, out var guildId))
        {
            return null;
        }

        return GetGuild(guildId);
    }

    public bool SetGuildMotd(int guildId, string motd)
    {
        if (!_guilds.TryGetValue(guildId, out var guild))
        {
            return false;
        }

        guild.Motd = motd;
        return true;
    }

    public bool DepositGold(int guildId, int amount)
    {
        if (!_guilds.TryGetValue(guildId, out var guild))
        {
            return false;
        }

        guild.GuildTreasury += amount;
        return true;
    }

    public bool WithdrawGold(int guildId, int amount)
    {
        if (!_guilds.TryGetValue(guildId, out var guild))
        {
            return false;
        }

        if (guild.GuildTreasury < amount)
        {
            return false;
        }

        guild.GuildTreasury -= amount;
        return true;
    }
}

