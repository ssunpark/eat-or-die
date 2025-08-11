using System.Linq;
using UnityEngine;

public class RecipePanelUIManager : BehaviourSingleton<RecipePanelUIManager>
{
    private ItemInstance[] _ingredients;
    public ItemInstance[] Ingredients => _ingredients;
    public UI_RecipeList RecipeListUI;
    
    private void Start()
    {
        InventoryManager.Instance.OnInventoryUpdated += UpdateIngredients;
    }
    
    public void UpdateIngredients()
    {
        Debug.Log("RecipePanelUIManager의 UpdateIngredients 메서드 진입");
        var validIngredientIDs = RecipeManager.Instance.RecipeList
            .Where(recipe => recipe.Ingredient2ID.HasValue) // 재료 2개짜리만
            .SelectMany(recipe => new[] { recipe.Ingredient1ID, recipe.Ingredient2ID.Value }) // 양쪽 다 꺼냄
            .Distinct()
            .ToHashSet();
        
        _ingredients = InventoryManager.Instance.Inventory.SlotList
            .Where(slot => slot.ItemInstance != null && slot.ItemInstance.ID >= 200000 && slot.ItemInstance.ID < 300000 && validIngredientIDs.Contains(slot.ItemInstance.ID))
            .Select(slot => slot.ItemInstance)
            .ToArray();
    }

    public void UpdateRecipes(int ingredientID)
    {
        var filteredRecipes = RecipeManager.Instance.RecipeList
            .Where(recipe => recipe.Ingredient2ID.HasValue)
            .Where(recipe => recipe.Ingredient1ID == ingredientID || recipe.Ingredient2ID == ingredientID)
            .ToList();

        Debug.Log($"[RecipePanel] Found {filteredRecipes.Count} recipes with Ingredient ID {ingredientID}");
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
    
        if (!InventoryManager.Instance.Inventory.HaveItem(ingredient1ID))
        {
            return false;
        }

        if (ingredient2ID.HasValue)
        {
            if (!InventoryManager.Instance.Inventory.HaveItem(ingredient2ID.Value))
            {
                return false;
            }
        }
        return true;
    }
}

