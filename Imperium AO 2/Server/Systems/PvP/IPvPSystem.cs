using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.PvP;

public interface IPvPSystem
{
    void FlagForPvP(int playerId);
    void UnflagPlayer(int playerId);
    void ReportKill(int killerId, int victimId);
    void ReportCriminalAct(int playerId);
    PvPFlag? GetPvPStatus(int playerId);
    bool CanAttack(int attackerId, int defenderId);
    void UpdatePvPFlags();
    int GetPlayerNotoriety(int playerId);
    List<int> GetPlayersWantedFor(int playerId);
}

