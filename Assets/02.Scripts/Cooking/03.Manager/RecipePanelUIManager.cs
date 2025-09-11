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
    private UI_CookingPanel _cookingPanel;
    
    private void Awake()
    {
        // 기본 카테고리를 Food로 설정
        _currentCategory = ERecipeCategory.Food;
    }
    
    public void OnCategoryButtonClick(int categoryIndex)
    {
        _currentCategory = (ERecipeCategory)categoryIndex;
        IngredientListUI.PopulateIngredients(_currentCategory);
        UpdateAllRecipes();
    }
    
    public void SetCookingPanel(UI_CookingPanel cookingPanel)
    {
        _cookingPanel = cookingPanel;
    }
    
    public void UpdateIngredientNameText(string text)
    {
        if (_cookingPanel != null && _cookingPanel.IngredientNameText != null)
        {
            _cookingPanel.IngredientNameText.text = text;
        }
    }
    
    private string GetCategoryDisplayName(ERecipeCategory category)
    {
        return category == ERecipeCategory.Food ? "음식 (전체)" : "무기 (전체)";
    }
    
    public void SetCurrentIngredientID(int ID)
    {
        CurrentIngredientID = ID;
    }

    public void UpdateAllRecipes()
    {
        var filteredRecipes = RecipeManager.Instance.GetRecipesByCategory(_currentCategory);
        RecipeListUI.ShowFilteredRecipes(filteredRecipes);
        
        // 전체 카테고리 텍스트 업데이트
        UpdateIngredientNameText(GetCategoryDisplayName(_currentCategory));
        CurrentIngredientID = 0; // 전체 보기로 리셋
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

