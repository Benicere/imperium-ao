using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Admin;

public interface IAdminSystem
{
    void BanPlayer(int playerId, string reason, int? durationSeconds = null);
    void UnbanPlayer(int playerId);
    void MutePlayer(int playerId, string reason, int durationSeconds);
    void UnmutePlayer(int playerId);
    bool IsPlayerBanned(int playerId);
    bool IsPlayerMuted(int playerId);
    void SetAdminLevel(int playerId, AdminLevel level);
    AdminLevel GetAdminLevel(int playerId);
    void ExecuteCommand(int adminId, AdminCommand command);
    void WarnPlayer(int playerId, string reason);
    List<Ban> GetActiveBans();
    List<Mute> GetActiveMutes();
    void KickPlayer(int playerId);
}

