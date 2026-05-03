using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Combat;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers;

public class ApplyEffectHandler : IPacketHandler
{
    private readonly IEffectSystem _effectSystem;
    private readonly ILogger<ApplyEffectHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Attack;

    public ApplyEffectHandler(IEffectSystem effectSystem, ILogger<ApplyEffectHandler> logger)
    {
        _effectSystem = effectSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var targetId = packet.GetInteger();
            var sourceId = packet.GetInteger();
            var effectTypeId = packet.GetByte();
            var damage = packet.GetInteger();
            var duration = packet.GetInteger();

            if (!Enum.TryParse<EffectType>(effectTypeId.ToString(), out var effectType))
            {
                _logger.LogWarning($"Invalid effect type: {effectTypeId}");
                return;
            }

            _effectSystem.ApplyEffect(targetId, sourceId, effectType, damage, duration);
            _logger.LogInformation($"Effect {effectType} applied to {targetId} by {sourceId}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling apply effect packet");
        }
    }
}

