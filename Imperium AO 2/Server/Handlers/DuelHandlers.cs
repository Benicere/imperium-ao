using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Duels;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ImperiumAO.Server.Handlers;

public class FightSendHandler : IPacketHandler
{
    private readonly IDuelSystem _duelSystem;
    private readonly ILogger<FightSendHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Attack;

    public FightSendHandler(
        IDuelSystem duelSystem,
        ILogger<FightSendHandler> logger)
    {
        _duelSystem = duelSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var targetId = packet.GetInteger();
            var targetName = packet.GetString();

            _duelSystem.CreateChallenge(connectionId, "Challenger", targetId, targetName);
            _logger.LogInformation($"Duel request sent: {connectionId} -> {targetName}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling fight send packet");
        }
    }
}

public class FightAcceptHandler : IPacketHandler
{
    private readonly IDuelSystem _duelSystem;
    private readonly ILogger<FightAcceptHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.Attack;

    public FightAcceptHandler(
        IDuelSystem duelSystem,
        ILogger<FightAcceptHandler> logger)
    {
        _duelSystem = duelSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var challengeId = packet.GetInteger();

            var challenge = _duelSystem.GetChallenge(challengeId);
            if (challenge == null)
            {
                return;
            }

            _duelSystem.AcceptChallenge(challengeId, connectionId);
            _duelSystem.StartDuel(challengeId);
            await Task.CompletedTask;

            _logger.LogInformation($"Duel {challengeId} accepted and started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling fight accept packet");
        }
    }
}

