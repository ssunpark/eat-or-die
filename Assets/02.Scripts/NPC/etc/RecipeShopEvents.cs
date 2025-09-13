using System;
using UnityEngine;

public static class RecipeShopEvents
{
    public static event Action<int> OnRecipeScrollUsed;

    public static void InvokeRecipeScrollUsed(int recipeID)
    {
        Debug.Log("RecipeShopEvents.InvokeRecipeScrollUsed");
        OnRecipeScrollUsed?.Invoke(recipeID);
    }
        
}