using System;
namespace ImperiumAO.Server.Systems.Inventory;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ItemType Type { get; set; }
    public Rarity Rarity { get; set; }
    public int Weight { get; set; }
    public int Value { get; set; }
    public int Quantity { get; set; } = 1;
    public int MaxStackSize { get; set; } = 64;
    public bool IsBindable { get; set; }
    public bool IsBound { get; set; }
    public int RequiredLevel { get; set; }
    public string Description { get; set; } = "";
}

public enum ItemType
{
    Equipment,
    Weapon,
    Armor,
    Consumable,
    Quest,
    Junk,
    Currency,
    Crafting
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

