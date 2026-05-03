using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Quests;

public interface IQuestSystem
{
    Quest? GetQuest(int questId);
    bool StartQuest(int characterId, int questId);
    bool UpdateObjective(int characterId, int questId, int objectiveId);
    bool CompleteQuest(int characterId, int questId);
    bool AbandonQuest(int characterId, int questId);
    PlayerQuest? GetPlayerQuest(int characterId, int questId);
    List<PlayerQuest> GetPlayerQuests(int characterId);
    List<Quest> GetAvailableQuests(int characterLevel);
    bool IsQuestCompleted(int characterId, int questId);
}

