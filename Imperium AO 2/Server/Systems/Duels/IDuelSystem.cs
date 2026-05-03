using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Duels;

public interface IDuelSystem
{
    DuelChallenge? CreateChallenge(int challengerId, string challengerName, int challengedId, string challengedName);
    DuelChallenge? GetChallenge(int challengeId);
    bool AcceptChallenge(int challengeId, int characterId);
    bool RejectChallenge(int challengeId, int characterId);
    bool StartDuel(int challengeId);
    bool EndDuel(int challengeId, int winnerId);
    List<DuelChallenge> GetPendingChallenges(int characterId);
    List<DuelChallenge> GetActiveDuels();
}

