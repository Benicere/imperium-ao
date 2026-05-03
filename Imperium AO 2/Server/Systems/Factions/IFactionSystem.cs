using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Factions;

public interface IFactionSystem
{
    void SetCharacterFaction(int characterId, Faction faction);
    Faction GetCharacterFaction(int characterId);
    void AddReputation(int characterId, int amount);
    int GetReputation(int characterId);
    FactionRank GetFactionRank(int characterId);
    bool CanInteractWithFaction(int characterId, Faction requiredFaction);
    List<int> GetCharactersByFaction(Faction faction);
    bool IsEnemy(int characterId1, int characterId2);
}

public enum Faction
{
    Real = 0,
    Chaos = 1,
    Neutral = 2
}

