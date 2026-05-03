using System;
using System.Collections.Generic;
using ImperiumAO.Server.Systems.Skills;

namespace ImperiumAO.Server.Systems.Crafting;

public class CraftingSystem : ICraftingSystem
{
    public bool CraftItem(int characterId, int recipeId) => true;
    public CraftingRecipe? GetRecipe(int recipeId) => null;
    public List<CraftingRecipe> GetRecipesByProfession(ProfessionType profession) => new();
    public bool CanCraftRecipe(int characterId, int recipeId) => true;
    public List<CraftingRecipe> GetAvailableRecipes(int characterId, int professionId) => new();
    public float GetSuccessRate(int characterId, CraftingRecipe recipe) => 1.0f;
    public int EstimateCraftingTime(CraftingRecipe recipe) => 0;
}
