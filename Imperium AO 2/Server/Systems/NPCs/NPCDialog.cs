using System;
using System.Collections.Generic;
using System.Linq;
namespace ImperiumAO.Server.Systems.NPCs;

public class DialogOption
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int NextDialogId { get; set; }
    public int RewardType { get; set; }
    public int RewardAmount { get; set; }
}

public class NPCDialog
{
    public int Id { get; set; }
    public int NpcId { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<DialogOption> Options { get; set; } = new();
    public int QuestId { get; set; }
    public bool IsQuestGiver { get; set; }
    public bool IsQuestCompleter { get; set; }
    public string? QuestName { get; set; }
}

public interface INPCDialogSystem
{
    void AddDialog(NPCDialog dialog);
    NPCDialog? GetDialog(int dialogId);
    NPCDialog? GetNPCDialog(int npcId);
    List<DialogOption> GetDialogOptions(int dialogId);
    void ProcessDialogChoice(int playerId, int npcId, int optionId);
}

public class NPCDialogSystem : INPCDialogSystem
{
    private readonly Dictionary<int, NPCDialog> _dialogs = new();
    private readonly Dictionary<int, Stack<int>> _playerDialogHistory = new();

    public void AddDialog(NPCDialog dialog)
    {
        _dialogs[dialog.Id] = dialog;
    }

    public NPCDialog? GetDialog(int dialogId)
    {
        _dialogs.TryGetValue(dialogId, out var dialog);
        return dialog;
    }

    public NPCDialog? GetNPCDialog(int npcId)
    {
        return _dialogs.Values.FirstOrDefault(d => d.NpcId == npcId);
    }

    public List<DialogOption> GetDialogOptions(int dialogId)
    {
        var dialog = GetDialog(dialogId);
        return dialog?.Options ?? new();
    }

    public void ProcessDialogChoice(int playerId, int npcId, int optionId)
    {
        if (!_playerDialogHistory.ContainsKey(playerId))
        {
            _playerDialogHistory[playerId] = new();
        }

        var npcDialog = GetNPCDialog(npcId);
        if (npcDialog != null)
        {
            var option = npcDialog.Options.FirstOrDefault(o => o.Id == optionId);
            if (option != null && option.NextDialogId > 0)
            {
                _playerDialogHistory[playerId].Push(option.NextDialogId);
            }
        }
    }
}

