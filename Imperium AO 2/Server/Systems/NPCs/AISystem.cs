using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.NPCs;

public class AISystem : IAISystem
{
    private readonly Dictionary<int, DateTime> _lastDecisionTime = new();
    private readonly Random _random = new();
    private readonly ILogger<AISystem> _logger;
    private const float DecisionInterval = 1.0f;

    public AISystem(ILogger<AISystem> logger)
    {
        _logger = logger;
    }

    public void UpdateAI(float deltaTime)
    {
    }

    public void ProcessNPCBehavior(NPC npc)
    {
        if (!_lastDecisionTime.ContainsKey(npc.Id))
        {
            _lastDecisionTime[npc.Id] = DateTime.UtcNow;
        }

        var timeSinceLastDecision = (DateTime.UtcNow - _lastDecisionTime[npc.Id]).TotalSeconds;
        if (timeSinceLastDecision >= DecisionInterval)
        {
            MakeDecision(npc);
            _lastDecisionTime[npc.Id] = DateTime.UtcNow;
        }
    }

    public bool ShouldAggro(NPC npc, int targetId, int targetX, int targetY)
    {
        if (!npc.IsAggressive)
        {
            return false;
        }

        var distance = Math.Sqrt(Math.Pow(npc.X - targetX, 2) + Math.Pow(npc.Y - targetY, 2));
        return distance <= npc.AggroRange;
    }

    public void MakeDecision(NPC npc)
    {
        switch (npc.Behavior)
        {
            case AIBehavior.Idle:
                IdleBehavior(npc);
                break;
            case AIBehavior.Patrolling:
                PatrolBehavior(npc);
                break;
            case AIBehavior.Chasing:
                ChaseBehavior(npc);
                break;
            case AIBehavior.Attacking:
                AttackBehavior(npc);
                break;
            case AIBehavior.Fleeing:
                FleeBehavior(npc);
                break;
        }
    }

    public List<int> GetVisibleTargets(NPC npc, List<int> nearbyCharacters)
    {
        return nearbyCharacters
            .Where(charId => {
                var distance = _random.Next(1, 20);
                return distance <= npc.AggroRange;
            })
            .ToList();
    }

    private void IdleBehavior(NPC npc)
    {
        var randomAction = _random.Next(0, 3);
        if (randomAction == 0)
        {
            npc.Behavior = AIBehavior.Patrolling;
            _logger.LogDebug($"NPC {npc.Name} started patrolling");
        }
    }

    private void PatrolBehavior(NPC npc)
    {
        var moveChance = _random.Next(0, 10);
        if (moveChance == 0)
        {
            var randomX = _random.Next(-2, 3);
            var randomY = _random.Next(-2, 3);

            npc.X += randomX;
            npc.Y += randomY;

            npc.X = Math.Max(0, Math.Min(100, npc.X));
            npc.Y = Math.Max(0, Math.Min(100, npc.Y));
        }

        var randomStop = _random.Next(0, 20);
        if (randomStop == 0)
        {
            npc.Behavior = AIBehavior.Idle;
            _logger.LogDebug($"NPC {npc.Name} stopped patrolling");
        }
    }

    private void ChaseBehavior(NPC npc)
    {
        if (!npc.CurrentTargetId.HasValue)
        {
            npc.Behavior = AIBehavior.Idle;
            return;
        }

        var moveChance = _random.Next(0, 5);
        if (moveChance == 0)
        {
            npc.X += _random.Next(-1, 2);
            npc.Y += _random.Next(-1, 2);
        }
    }

    private void AttackBehavior(NPC npc)
    {
        if (!npc.CurrentTargetId.HasValue || npc.Stats.Health <= 0)
        {
            npc.Behavior = AIBehavior.Idle;
            npc.CurrentTargetId = null;
            return;
        }

        var attackChance = _random.Next(0, 10);
        if (attackChance < 3)
        {
            _logger.LogDebug($"NPC {npc.Name} is attacking target {npc.CurrentTargetId}");
        }
    }

    private void FleeBehavior(NPC npc)
    {
        if (npc.Stats.Health > npc.Stats.MaxHealth / 2)
        {
            npc.Behavior = AIBehavior.Attacking;
            return;
        }

        var fleeChance = _random.Next(0, 5);
        if (fleeChance == 0)
        {
            npc.X += _random.Next(-2, 3);
            npc.Y += _random.Next(-2, 3);
        }
    }
}

