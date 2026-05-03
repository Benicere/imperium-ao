using ImperiumAO.Server.Systems.Inventory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Trading;

public class TradingSystem : ITradingSystem
{
    private readonly Dictionary<int, TradeOffer> _offers = new();
    private int _nextOfferId = 1;
    private readonly ILogger<TradingSystem> _logger;

    public TradingSystem(ILogger<TradingSystem> logger)
    {
        _logger = logger;
    }

    public TradeOffer? CreateTradeOffer(int initiatorId, int targetId)
    {
        var offer = new TradeOffer
        {
            Id = _nextOfferId++,
            InitiatorId = initiatorId,
            TargetId = targetId,
            Status = TradeStatus.Pending
        };

        _offers[offer.Id] = offer;
        _logger.LogInformation($"Trade offer {offer.Id} created between {initiatorId} and {targetId}");
        return offer;
    }

    public bool AddItemToOffer(int offerId, int characterId, Item item)
    {
        if (!_offers.TryGetValue(offerId, out var offer))
        {
            _logger.LogWarning($"Offer {offerId} not found");
            return false;
        }

        if (characterId == offer.InitiatorId)
        {
            offer.InitiatorItems.Add(item);
            _logger.LogInformation($"Item added to offer {offerId} by initiator");
            return true;
        }
        else if (characterId == offer.TargetId)
        {
            offer.TargetItems.Add(item);
            _logger.LogInformation($"Item added to offer {offerId} by target");
            return true;
        }

        return false;
    }

    public bool RemoveItemFromOffer(int offerId, int characterId, int itemId)
    {
        if (!_offers.TryGetValue(offerId, out var offer))
        {
            return false;
        }

        List<Item> items = characterId == offer.InitiatorId
            ? offer.InitiatorItems
            : characterId == offer.TargetId
                ? offer.TargetItems
                : null;

        if (items == null)
        {
            return false;
        }

        var item = items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }

        return false;
    }

    public bool AddGoldToOffer(int offerId, int characterId, int amount)
    {
        if (!_offers.TryGetValue(offerId, out var offer))
        {
            return false;
        }

        if (characterId == offer.InitiatorId)
        {
            offer.InitiatorGold += amount;
            return true;
        }
        else if (characterId == offer.TargetId)
        {
            offer.TargetGold += amount;
            return true;
        }

        return false;
    }

    public bool AcceptOffer(int offerId, int characterId)
    {
        if (!_offers.TryGetValue(offerId, out var offer))
        {
            return false;
        }

        if (characterId == offer.InitiatorId)
        {
            offer.InitiatorAccepted = true;
        }
        else if (characterId == offer.TargetId)
        {
            offer.TargetAccepted = true;
        }
        else
        {
            return false;
        }

        if (offer.InitiatorAccepted && offer.TargetAccepted)
        {
            offer.Status = TradeStatus.Accepted;
            _logger.LogInformation($"Trade offer {offerId} has been accepted by both parties");
        }

        return true;
    }

    public bool CompleteOffer(int offerId)
    {
        if (!_offers.TryGetValue(offerId, out var offer))
        {
            return false;
        }

        offer.Status = TradeStatus.Completed;
        _logger.LogInformation($"Trade offer {offerId} completed");
        return true;
    }

    public bool RejectOffer(int offerId)
    {
        if (!_offers.TryGetValue(offerId, out var offer))
        {
            return false;
        }

        offer.Status = TradeStatus.Rejected;
        _logger.LogInformation($"Trade offer {offerId} rejected");
        return true;
    }

    public bool CancelOffer(int offerId)
    {
        if (!_offers.TryGetValue(offerId, out var offer))
        {
            return false;
        }

        offer.Status = TradeStatus.Cancelled;
        _offers.Remove(offerId);
        _logger.LogInformation($"Trade offer {offerId} cancelled");
        return true;
    }

    public TradeOffer? GetOffer(int offerId)
    {
        _offers.TryGetValue(offerId, out var offer);
        return offer;
    }

    public List<TradeOffer> GetPendingOffers(int characterId)
    {
        return _offers.Values
            .Where(o => (o.InitiatorId == characterId || o.TargetId == characterId) && o.Status == TradeStatus.Pending)
            .ToList();
    }
}

