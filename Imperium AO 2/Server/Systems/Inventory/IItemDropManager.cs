using System;
using System.Collections.Generic;
namespace ImperiumAO.Server.Systems.Inventory;

public interface IItemDropManager
{
    void DropItem(int playerId, int itemId, int x, int y, int z);
    void PickupItem(int playerId, int dropId);
    List<ItemDrop> GetItemsAt(int x, int y, int z);
    ItemDrop? GetDropById(int dropId);
    void RemoveDrop(int dropId);
    void ClearExpiredDrops();
}

