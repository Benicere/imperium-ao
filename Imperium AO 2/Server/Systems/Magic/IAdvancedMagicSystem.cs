using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Magic;

public interface IAdvancedMagicSystem
{
    void LearnSpell(int playerId, AdvancedSpell spell);
    void ForgetSpell(int playerId, int spellId);
    AdvancedSpell? GetSpell(int spellId);
    List<AdvancedSpell> GetPlayerSpells(int playerId);
    List<AdvancedSpell> GetSpellsBySchool(int playerId, SpellSchool school);
    bool CanCastSpell(int playerId, int spellId);
    void CastSpell(int playerId, int spellId, int targetId);
    void GainSchoolExperience(int playerId, SpellSchool school, int amount);
    SpellSchoolProgression? GetSchoolProgression(int playerId, SpellSchool school);
    List<AdvancedSpell> GetAoeSpells();
}

