using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Quests;

public class QuestSystem : IQuestSystem
{
    private readonly Dictionary<int, Quest> _quests = new();
    private readonly Dictionary<int, List<PlayerQuest>> _playerQuests = new();
    private readonly ILogger<QuestSystem> _logger;

    public QuestSystem(ILogger<QuestSystem> logger)
    {
        _logger = logger;
        InitializeDefaultQuests();
    }

    public Quest? GetQuest(int questId)
    {
        _quests.TryGetValue(questId, out var quest);
        return quest;
    }

    public bool StartQuest(int characterId, int questId)
    {
        var quest = GetQuest(questId);
        if (quest == null)
        {
            return false;
        }

        if (!_playerQuests.ContainsKey(characterId))
        {
            _playerQuests[characterId] = new();
        }

        var playerQuest = new PlayerQuest
        {
            CharacterId = characterId,
            QuestId = questId,
            Status = QuestStatus.InProgress,
            StartedAt = DateTime.UtcNow
        };

        _playerQuests[characterId].Add(playerQuest);
        _logger.LogInformation($"Character {characterId} started quest {questId}");
        return true;
    }

    public bool UpdateObjective(int characterId, int questId, int objectiveId)
    {
        var playerQuest = GetPlayerQuest(characterId, questId);
        if (playerQuest == null)
        {
            return false;
        }

        if (!playerQuest.ObjectiveProgress.ContainsKey(objectiveId))
        {
            playerQuest.ObjectiveProgress[objectiveId] = 0;
        }

        playerQuest.ObjectiveProgress[objectiveId]++;
        _logger.LogInformation($"Character {characterId} progressed objective {objectiveId} in quest {questId}");
        return true;
    }

    public bool CompleteQuest(int characterId, int questId)
    {
        var playerQuest = GetPlayerQuest(characterId, questId);
        if (playerQuest == null)
        {
            return false;
        }

        playerQuest.Status = QuestStatus.Completed;
        playerQuest.CompletedAt = DateTime.UtcNow;
        _logger.LogInformation($"Character {characterId} completed quest {questId}");
        return true;
    }

    public bool AbandonQuest(int characterId, int questId)
    {
        var playerQuest = GetPlayerQuest(characterId, questId);
        if (playerQuest == null)
        {
            return false;
        }

        playerQuest.Status = QuestStatus.Abandoned;
        _logger.LogInformation($"Character {characterId} abandoned quest {questId}");
        return true;
    }

    public PlayerQuest? GetPlayerQuest(int characterId, int questId)
    {
        if (!_playerQuests.TryGetValue(characterId, out var quests))
        {
            return null;
        }

        return quests.FirstOrDefault(q => q.QuestId == questId);
    }

    public List<PlayerQuest> GetPlayerQuests(int characterId)
    {
        if (!_playerQuests.TryGetValue(characterId, out var quests))
        {
            return new();
        }

        return quests.ToList();
    }

    public List<Quest> GetAvailableQuests(int characterLevel)
    {
        return _quests.Values
            .Where(q => q.RequiredLevel <= characterLevel)
            .ToList();
    }

    public bool IsQuestCompleted(int characterId, int questId)
    {
        var playerQuest = GetPlayerQuest(characterId, questId);
        return playerQuest?.Status == QuestStatus.Completed;
    }

    private void InitializeDefaultQuests()
    {
        var quest1 = new Quest
        {
            Id = 1,
            Name = "First Steps",
            Description = "A beginner's quest",
            RewardExp = 100,
            RewardGold = 50,
            RequiredLevel = 1,
            Type = QuestType.Kill,
            GiverNpcId = 1,
            Objectives = new()
            {
                new QuestObjective { Id = 1, Description = "Kill 5 rats", Type = ObjectiveType.KillMonster, TargetId = 1, RequiredProgress = 5 }
            }
        };

        _quests[1] = quest1;
    }
}

