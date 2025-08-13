using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookPayload : ISkillPayload, IItemPayload
{
    public readonly int CookedId;
    public List<int> IngredientIds = new();

    public CookPayload(int cookedId)
    {
        CookedId = cookedId;
        Recipe recipe = RecipeManager.Instance.RecipeList.First(x => x.ResultID == CookedId);
        IngredientIds.Add(recipe.Ingredient1ID);
        if (recipe.Ingredient2ID != null)
        {
            IngredientIds.Add(recipe.Ingredient2ID ?? recipe.Ingredient1ID);
        }
    }

    private int GetRandomIngredients()
    {
        return Random.Range(0, IngredientIds.Count);
    }

    public int ItemId => GetRandomIngredients();
}