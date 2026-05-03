using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Combat;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ImperiumAO.Server.Handlers;

public class CastSpellHandler : IPacketHandler
{
    private readonly ISpellSystem _spellSystem;
    private readonly ICombatCalculator _combatCalculator;
    private readonly ILogger<CastSpellHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.CastSpell;

    public CastSpellHandler(
        ISpellSystem spellSystem,
        ICombatCalculator combatCalculator,
        ILogger<CastSpellHandler> logger)
    {
        _spellSystem = spellSystem;
        _combatCalculator = combatCalculator;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var spellId = packet.GetInteger();
            var targetId = packet.GetInteger();

            _spellSystem.CastSpell(connectionId, targetId, null);
            _logger.LogInformation($"Spell cast: {connectionId} -> {targetId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling cast spell packet");
        }
    }
}


