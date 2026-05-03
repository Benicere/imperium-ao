using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Statistics;

public interface IStatisticsSystem
{
    void RecordPlayerLogin(int playerId, string playerName);
    void RecordPlayerLogout(int playerId);
    void RecordKill(int killerId, int victimId);
    void RecordDeath(int playerId);
    void RecordQuestCompletion(int playerId);
    void RecordItemCraft(int playerId);
    void RecordTrade(int playerId1, int playerId2);
    void RecordGoldEarned(int playerId, int amount);
    void RecordGoldSpent(int playerId, int amount);
    PlayerStatistics? GetPlayerStatistics(int playerId);
    GameStatistics GetGameStatistics();
    List<PlayerStatistics> GetTopKillers(int count);
    List<PlayerStatistics> GetTopLevelPlayers(int count);
    List<PlayerStatistics> GetMostActivePlayers(int count);
}

