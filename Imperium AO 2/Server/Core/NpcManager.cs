using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImperiumAO.Common.Entities;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server;

public class NpcManager : INpcManager
{
    private readonly ILogger<NpcManager> _logger;
    private readonly List<Npc> _npcs = new();

    public NpcManager(ILogger<NpcManager> logger)
    {
        _logger = logger;
    }

    public Task InitializeAsync()
    {
        _logger.LogInformation("Initializing NPCs");
        return Task.CompletedTask;
    }

    public IReadOnlyList<Npc> GetNpcs()
    {
        return _npcs.AsReadOnly();
    }
}
