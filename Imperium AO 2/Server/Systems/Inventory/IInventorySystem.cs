using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Inventory;

public interface IInventorySystem
{
    bool AddItem(int characterId, Item item);
    bool RemoveItem(int characterId, int itemId, int quantity = 1);
    Item? GetItem(int characterId, int itemId);
    List<Item> GetInventory(int characterId);
    bool UseItem(int characterId, int itemId);
    bool MoveItem(int characterId, int itemId, int newSlot);
    bool HasSpace(int characterId);
    int GetInventoryWeight(int characterId);
    int GetMaxInventoryWeight();
}

