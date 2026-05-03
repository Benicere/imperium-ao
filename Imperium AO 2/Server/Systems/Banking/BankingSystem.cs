using ImperiumAO.Server.Systems.Inventory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Banking;

public class BankingSystem : IBankingSystem
{
    private readonly Dictionary<int, BankAccount> _accounts = new();
    private readonly ILogger<BankingSystem> _logger;

    public BankingSystem(ILogger<BankingSystem> logger)
    {
        _logger = logger;
    }

    public BankAccount? GetBankAccount(int characterId)
    {
        _accounts.TryGetValue(characterId, out var account);
        return account;
    }

    public bool CreateBankAccount(int characterId)
    {
        if (_accounts.ContainsKey(characterId))
        {
            _logger.LogWarning($"Bank account already exists for character {characterId}");
            return false;
        }

        var account = new BankAccount { CharacterId = characterId };
        _accounts[characterId] = account;

        _logger.LogInformation($"Bank account created for character {characterId}");
        return true;
    }

    public bool DepositGold(int characterId, long amount)
    {
        var account = GetBankAccount(characterId);
        if (account == null)
        {
            _logger.LogWarning($"Bank account not found for character {characterId}");
            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        account.Gold += amount;
        account.LastAccessedAt = DateTime.UtcNow;

        _logger.LogInformation($"Character {characterId} deposited {amount} gold (Total: {account.Gold})");
        return true;
    }

    public bool WithdrawGold(int characterId, long amount)
    {
        var account = GetBankAccount(characterId);
        if (account == null)
        {
            return false;
        }

        if (amount <= 0 || account.Gold < amount)
        {
            return false;
        }

        account.Gold -= amount;
        account.LastAccessedAt = DateTime.UtcNow;

        _logger.LogInformation($"Character {characterId} withdrew {amount} gold (Total: {account.Gold})");
        return true;
    }

    public bool DepositItem(int characterId, Item item)
    {
        var account = GetBankAccount(characterId);
        if (account == null)
        {
            return false;
        }

        if (!HasSpace(characterId))
        {
            _logger.LogWarning($"Bank full for character {characterId}");
            return false;
        }

        account.Items.Add(item);
        account.LastAccessedAt = DateTime.UtcNow;

        _logger.LogInformation($"Character {characterId} deposited {item.Name}");
        return true;
    }

    public bool WithdrawItem(int characterId, int itemId, int quantity = 1)
    {
        var account = GetBankAccount(characterId);
        if (account == null)
        {
            return false;
        }

        var item = account.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return false;
        }

        if (item.Quantity >= quantity)
        {
            item.Quantity -= quantity;
            if (item.Quantity == 0)
            {
                account.Items.Remove(item);
            }
            account.LastAccessedAt = DateTime.UtcNow;

            _logger.LogInformation($"Character {characterId} withdrew {quantity} of {item.Name}");
            return true;
        }

        return false;
    }

    public List<Item> GetBankItems(int characterId)
    {
        var account = GetBankAccount(characterId);
        if (account == null)
        {
            return new();
        }

        return account.Items.ToList();
    }

    public long GetBankGold(int characterId)
    {
        var account = GetBankAccount(characterId);
        return account?.Gold ?? 0;
    }

    public bool HasSpace(int characterId)
    {
        var account = GetBankAccount(characterId);
        if (account == null)
        {
            return false;
        }

        return account.Items.Count < account.MaxSlots;
    }
}


