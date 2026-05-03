using System;
using ImperiumAO.Common.Entities;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server;

public class CombatSystem : ICombatSystem
{
    private readonly ILogger<CombatSystem> _logger;

    public CombatSystem(ILogger<CombatSystem> logger)
    {
        _logger = logger;
    }

    public void Attack(Character attacker, Character target)
    {
        if (!attacker.IsAlive() || !target.IsAlive())
        {
            _logger.LogDebug("Cannot attack with dead character");
            return;
        }

        var damage = CalculateDamage(attacker, target);
        TakeDamage(target, damage);

        _logger.LogDebug("{Attacker} attacked {Target} for {Damage} damage",
            attacker.Name, target.Name, damage);
    }

    public void TakeDamage(Character target, int damage)
    {
        target.Health -= damage;
        if (target.Health < 0)
            target.Health = 0;
    }

    public bool IsDead(Character character)
    {
        return character.Health <= 0;
    }

    private int CalculateDamage(Character attacker, Character target)
    {
        var baseDamage = 10;
        var variance = Random.Shared.Next(-2, 3);
        return Math.Max(1, baseDamage + variance);
    }
}
