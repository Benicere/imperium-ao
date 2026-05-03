using ImperiumAO.Server.Systems.Inventory;
using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Banking;

public interface IBankingSystem
{
    BankAccount? GetBankAccount(int characterId);
    bool CreateBankAccount(int characterId);
    bool DepositGold(int characterId, long amount);
    bool WithdrawGold(int characterId, long amount);
    bool DepositItem(int characterId, Item item);
    bool WithdrawItem(int characterId, int itemId, int quantity = 1);
    List<Item> GetBankItems(int characterId);
    long GetBankGold(int characterId);
    bool HasSpace(int characterId);
}


