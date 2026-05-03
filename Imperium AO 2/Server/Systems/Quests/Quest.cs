using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Quests;

public class Quest
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int RewardExp { get; set; }
    public int RewardGold { get; set; }
    public int RequiredLevel { get; set; }
    public QuestType Type { get; set; }
    public int GiverNpcId { get; set; }
    public List<QuestObjective> Objectives { get; set; } = new();
    public List<int> RewardItemIds { get; set; } = new();
}

public class QuestObjective
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public ObjectiveType Type { get; set; }
    public int TargetId { get; set; }
    public int CurrentProgress { get; set; }
    public int RequiredProgress { get; set; }
}

public class PlayerQuest
{
    public int CharacterId { get; set; }
    public int QuestId { get; set; }
    public QuestStatus Status { get; set; } = QuestStatus.NotStarted;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Dictionary<int, int> ObjectiveProgress { get; set; } = new();
}

public enum QuestType
{
    Kill,
    Collect,
    Deliver,
    Explore,
    Talk,
    Craft
}

public enum ObjectiveType
{
    KillMonster,
    CollectItem,
    DeliverItem,
    ReachLocation,
    TalkToNpc,
    CraftItem
}

public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed,
    Abandoned,
    Failed
}

