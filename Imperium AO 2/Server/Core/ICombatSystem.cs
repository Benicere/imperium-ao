using ImperiumAO.Common.Entities;
using System;

namespace ImperiumAO.Server;

public interface ICombatSystem
{
    void Attack(Character attacker, Character target);
    void TakeDamage(Character target, int damage);
    bool IsDead(Character character);
}

