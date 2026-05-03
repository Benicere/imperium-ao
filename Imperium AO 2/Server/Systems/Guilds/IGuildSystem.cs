using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Guilds;

public interface IGuildSystem
{
    Guild? CreateGuild(string name, int leaderId);
    Guild? GetGuild(int guildId);
    Guild? GetGuildByName(string name);
    bool AddMember(int guildId, int characterId, string characterName);
    bool RemoveMember(int guildId, int characterId);
    bool PromoteMember(int guildId, int characterId, GuildRank newRank);
    bool DisbandGuild(int guildId);
    List<Guild> GetAllGuilds();
    Guild? GetCharacterGuild(int characterId);
    bool SetGuildMotd(int guildId, string motd);
    bool DepositGold(int guildId, int amount);
    bool WithdrawGold(int guildId, int amount);
}

