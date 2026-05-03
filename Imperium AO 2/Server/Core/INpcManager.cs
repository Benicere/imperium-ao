using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImperiumAO.Common.Entities;

namespace ImperiumAO.Server;

public interface INpcManager
{
    Task InitializeAsync();
    IReadOnlyList<Npc> GetNpcs();
}
