using ImperiumAO.Server.Systems.Inventory;
using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Crafting;

public interface ICraftingSystem
{
    CraftingRecipe? GetRecipe(int recipeId);
    List<CraftingRecipe> GetRecipesByProfession(ProfessionType profession);
    bool CanCraftRecipe(int characterId, int recipeId);
    bool CraftItem(int characterId, int recipeId);
    List<CraftingRecipe> GetAvailableRecipes(int characterId, int professionLevel);
    float GetSuccessRate(int characterId, CraftingRecipe recipe);
    int EstimateCraftingTime(CraftingRecipe recipe);
}

