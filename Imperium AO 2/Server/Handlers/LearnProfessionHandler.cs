using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Skills;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers;

public class LearnProfessionHandler : IPacketHandler
{
    private readonly IProfessionSystem _professionSystem;
    private readonly ILogger<LearnProfessionHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Train;

    public LearnProfessionHandler(IProfessionSystem professionSystem, ILogger<LearnProfessionHandler> logger)
    {
        _professionSystem = professionSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var professionTypeId = packet.GetByte();

            if (!Enum.TryParse<ProfessionType>(professionTypeId.ToString(), out var professionType))
            {
                _logger.LogWarning($"Invalid profession type: {professionTypeId}");
                return;
            }

            _professionSystem.LearnProfession(connectionId, professionType);
            _logger.LogInformation($"Player {connectionId} learned profession {professionType}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling learn profession packet");
        }
    }
}
