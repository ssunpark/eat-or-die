using System.Linq;
using UnityEngine;

public enum ERecipeCategory
{
    Food,
    Weapon
}


public class RecipePanelUIManager : BehaviourSingleton<RecipePanelUIManager>
{
    public UI_RecipeList RecipeListUI;
    public UI_RecipeIngredient IngredientListUI; // ★ Ingredient UI 참조 추가
    public int CurrentIngredientID;

    
    // ★ 현재 선택된 카테고리를 저장할 변수 추가
    private ERecipeCategory _currentCategory;

    // ★ UI의 카테고리 버튼 (Food, Weapon)에서 이 메서드를 호출하도록 연결합니다.
    public void OnCategoryButtonClick(int categoryIndex)
    {
        _currentCategory = (ERecipeCategory)categoryIndex;

        // 1. 재료 리스트를 현재 카테고리에 맞게 새로 고칩니다.
        IngredientListUI.PopulateIngredients(_currentCategory);

        // 2. '전체' 버튼을 누른 것처럼 해당 카테고리의 모든 레시피를 표시합니다.
        UpdateAllRecipes();
    }
    
    public void SetCurrentIngredientID(int ID)
    {
        CurrentIngredientID = ID;
    }

    public void UpdateAllRecipes()
    {
        // ★ 현재 카테고리에 맞는 모든 레시피를 가져옵니다. (RecipeManager에 유사한 기능이 필요)
        var filteredRecipes = RecipeManager.Instance.GetRecipesByCategory(_currentCategory);
        RecipeListUI.ShowFilteredRecipes(filteredRecipes);
    }

    public void UpdateRecipes()
    {
        // ★ 현재 카테고리에 맞는 레시피 중에서 재료와 관련된 것을 필터링합니다.
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

