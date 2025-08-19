using System.Linq;
using UnityEngine;

public class RecipePanelUIManager : BehaviourSingleton<RecipePanelUIManager>
{
    public UI_RecipeList RecipeListUI;
    public int CurrentIngredientID;

    public void SetCurrentIngredientID(int ID)
    {
        CurrentIngredientID = ID;
    }

    public void UpdateAllRecipes()
    {
        var filteredRecipes = RecipeManager.Instance.RecipeList;

        Debug.Log($"[RecipePanel] Found {filteredRecipes.Count} recipes with Ingredient ID {CurrentIngredientID}");
        RecipeListUI.ShowFilteredRecipes(filteredRecipes);
    }

    public void UpdateRecipes()
    {
        var filteredRecipes = RecipeManager.Instance.RecipeList
            .Where(recipe => recipe.Ingredient2ID.HasValue)
            .Where(recipe => recipe.Ingredient1ID == CurrentIngredientID || recipe.Ingredient2ID == CurrentIngredientID)
            .ToList();

        Debug.Log($"[RecipePanel] Found {filteredRecipes.Count} recipes with Ingredient ID {CurrentIngredientID}");
        RecipeListUI.ShowFilteredRecipes(filteredRecipes);
    }

    public bool IsKnownIngredient(int ingredientID)
    {
        return RoomRecipeStateManager.Instance.IsUnlockedIngredients(ingredientID);
    }

    public bool IsKnownRecipe(int recipeID)
    {
        return RoomRecipeStateManager.Instance.IsUnlockedRecipes(recipeID);
    }

    public bool CanMakeRecipe(Recipe recipe)
    {
        int ingredient1ID = recipe.Ingredient1ID;
        int? ingredient2ID = recipe.Ingredient2ID;

        // 재료 1개짜리 레시피
        if (!ingredient2ID.HasValue)
        {
            return UnifiedInventoryManager.Instance.HaveItem(ingredient1ID);
        }

        // 재료 2개인데 같은 재료
        if (ingredient1ID == ingredient2ID.Value)
        {
            Debug.Log("재료 2개인데 같은 재료임");
            return UnifiedInventoryManager.Instance.GetItemCount(ingredient1ID) >= 2;
        }

        // 재료 2개인데 서로 다른 경우
        else
        {
            return UnifiedInventoryManager.Instance.HaveItem(ingredient1ID) &&
                   UnifiedInventoryManager.Instance.HaveItem(ingredient2ID.Value);
        }
    }

    public void ActiveHoverUI()
    {
        Debug.Log("ActiveHoverUI");
    }

    public void DeactiveHoverUI()
    {
        Debug.Log("DeactiveHoverUI");
    }
}

