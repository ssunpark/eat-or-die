using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum ERecipeCategory
{
    Food,
    Weapon
}


public class RecipePanelUIManager : BehaviourSingleton<RecipePanelUIManager>
{
    public UI_RecipeList RecipeListUI;
    public UI_RecipeIngredient IngredientListUI;
    public int CurrentIngredientID;
    
    private ERecipeCategory _currentCategory;
    
    public void OnCategoryButtonClick(int categoryIndex)
    {
        _currentCategory = (ERecipeCategory)categoryIndex;
        IngredientListUI.PopulateIngredients(_currentCategory);
        UpdateAllRecipes();
    }
    
    public void SetCurrentIngredientID(int ID)
    {
        CurrentIngredientID = ID;
    }

    public void UpdateAllRecipes()
    {
        var filteredRecipes = RecipeManager.Instance.GetRecipesByCategory(_currentCategory);
        RecipeListUI.ShowFilteredRecipes(filteredRecipes);
    }

    public void UpdateRecipes()
    {
        var filteredRecipes = RecipeManager.Instance.GetRecipesByCategory(_currentCategory)
            .Where(recipe => recipe.Ingredient2ID.HasValue)
            .Where(recipe => recipe.Ingredient1ID == CurrentIngredientID || recipe.Ingredient2ID == CurrentIngredientID)
            .ToList();

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
            return UnifiedInventoryManager.Instance.GetItemCount(ingredient1ID) >= 2;
        }

        // 재료 2개인데 서로 다른 경우
        else
        {
            return UnifiedInventoryManager.Instance.HaveItem(ingredient1ID) &&
                   UnifiedInventoryManager.Instance.HaveItem(ingredient2ID.Value);
        }
    }
}

