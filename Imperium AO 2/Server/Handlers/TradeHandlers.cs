using System;
using System.Threading.Tasks;
using ImperiumAO.Common.Network;
using ImperiumAO.Server.Network;
using ImperiumAO.Server.Systems.Trading;
using Microsoft.Extensions.Logging;

namespace ImperiumAO.Server.Handlers;

public class InitiateTradeHandler : IPacketHandler
{
    private readonly ITradingSystem _tradingSystem;
    private readonly TradeValidator _validator;
    private readonly ILogger<InitiateTradeHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.CommerceStart;

    public InitiateTradeHandler(ITradingSystem tradingSystem, TradeValidator validator, ILogger<InitiateTradeHandler> logger)
    {
        _tradingSystem = tradingSystem;
        _validator = validator;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var targetId = packet.GetInteger();
            var goldAmount = packet.GetInteger();

            var offer = _tradingSystem.CreateTradeOffer(connectionId, targetId);
            if (offer == null)
            {
                _logger.LogWarning($"Failed to create trade offer");
                return;
            }

            if (goldAmount > 0)
            {
                _tradingSystem.AddGoldToOffer(offer.Id, connectionId, goldAmount);
            }

            _logger.LogInformation($"Trade initiated between {connectionId} and {targetId}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling initiate trade packet");
        }
    }
}

public class AcceptTradeHandler : IPacketHandler
{
    private readonly ITradingSystem _tradingSystem;
    private readonly ILogger<AcceptTradeHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.CommerceStart;

    public AcceptTradeHandler(ITradingSystem tradingSystem, ILogger<AcceptTradeHandler> logger)
    {
        _tradingSystem = tradingSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var offerId = packet.GetInteger();

            var offer = _tradingSystem.GetOffer(offerId);
            if (offer != null)
            {
                _tradingSystem.AcceptOffer(offerId, connectionId);
                _logger.LogInformation($"Trade {offerId} accepted by {connectionId}");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling accept trade packet");
        }
    }
}

public class CancelTradeHandler : IPacketHandler
{
    private readonly ITradingSystem _tradingSystem;
    private readonly ILogger<CancelTradeHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.CommerceStart;

    public CancelTradeHandler(ITradingSystem tradingSystem, ILogger<CancelTradeHandler> logger)
    {
        _tradingSystem = tradingSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var offerId = packet.GetInteger();

            _tradingSystem.CancelOffer(offerId);
            _logger.LogInformation($"Trade {offerId} canceled by {connectionId}");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling cancel trade packet");
        }
    }
}

public class CompleteTradeHandler : IPacketHandler
{
    private readonly ITradingSystem _tradingSystem;
    private readonly ILogger<CompleteTradeHandler> _logger;

    public ClientPacketId PacketId => ClientPacketId.CommerceStart;

    public CompleteTradeHandler(ITradingSystem tradingSystem, ILogger<CompleteTradeHandler> logger)
    {
        _tradingSystem = tradingSystem;
        _logger = logger;
    }

    public async Task HandleAsync(int connectionId, ByteBuffer packet, IGameConnection connection)
    {
        try
        {
            var offerId = packet.GetInteger();

            var offer = _tradingSystem.GetOffer(offerId);
            if (offer != null)
            {
                _tradingSystem.CompleteOffer(offerId);
                _logger.LogInformation($"Trade {offerId} completed");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling complete trade packet");
        }
    }
}
