using ImperiumAO.Server.Systems.Inventory;
using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Trading;

public interface ITradingSystem
{
    TradeOffer? CreateTradeOffer(int initiatorId, int targetId);
    bool AddItemToOffer(int offerId, int characterId, Item item);
    bool RemoveItemFromOffer(int offerId, int characterId, int itemId);
    bool AddGoldToOffer(int offerId, int characterId, int amount);
    bool AcceptOffer(int offerId, int characterId);
    bool CompleteOffer(int offerId);
    bool RejectOffer(int offerId);
    bool CancelOffer(int offerId);
    TradeOffer? GetOffer(int offerId);
    List<TradeOffer> GetPendingOffers(int characterId);
}

