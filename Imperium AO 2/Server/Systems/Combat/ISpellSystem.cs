using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Combat;

public interface ISpellSystem
{
    Spell? GetSpell(int spellId);
    List<Spell> GetCharacterSpells(int characterId);
    bool LearnSpell(int characterId, int spellId);
    bool ForgetSpell(int characterId, int spellId);
    bool CanCastSpell(int characterId, Spell spell);
    void CastSpell(int characterId, int targetId, Spell spell);
}

