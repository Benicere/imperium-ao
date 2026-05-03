using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Skills;

public interface IProfessionSystem
{
    void LearnProfession(int playerId, ProfessionType type);
    void GainProfessionExp(int playerId, ProfessionType type, int amount);
    Profession? GetProfession(int playerId, ProfessionType type);
    List<Profession> GetPlayerProfessions(int playerId);
    bool CanCraft(int playerId, ProfessionType type);
    void RemoveProfession(int playerId, ProfessionType type);
}

