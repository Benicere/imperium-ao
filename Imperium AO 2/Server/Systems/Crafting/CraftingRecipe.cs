using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Crafting;

public class CraftingRecipe
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ProfessionType Profession { get; set; }
    public int RequiredLevel { get; set; }
    public int CraftingTime { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public int ResultItemId { get; set; }
    public int ResultQuantity { get; set; } = 1;
    public float SuccessRate { get; set; } = 0.9f;
}

public class RecipeIngredient
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string ItemName { get; set; } = "";
}

public enum ProfessionType
{
    Blacksmith,
    Carpenter,
    Tailor,
    Alchemist,
    Jeweler,
    Leatherworker,
    Enchanter
}

