using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipePanelUIManager : BehaviourSingleton<RecipePanelUIManager>
{
    private Item[] _ingredients ;
    public Item[] Ingredients => _ingredients;
    public event Action OnInventoryUpdated;

    public UI_RecipeList RecipeListUI;
    private void Start()
    {
        InventoryManager.Instance.OnInventoryUpdated += UpdateIngredients;
    }
    
    // InventoryManager에 등록된 재료를 조건(ID)으로 필터
    // 디버그로 확인하고 이벤트 호출
    public void UpdateIngredients()
    {
        var validIngredientIDs = RecipeManager.Instance.RecipeList
            .Where(recipe => recipe.Ingredient2ID.HasValue) // 재료 2개짜리만
            .SelectMany(recipe => new[] { recipe.Ingredient1ID, recipe.Ingredient2ID.Value }) // 양쪽 다 꺼냄
            .Distinct()
            .ToHashSet();
        
        _ingredients = InventoryManager.Instance.Inventory.SlotList
            .Where(slot => slot.Item != null && slot.Item.ID >= 200000 && slot.Item.ID < 300000 && validIngredientIDs.Contains(slot.Item.ID))
            .Select(slot => slot.Item)
            .ToArray();
        for (int i = 0; i < _ingredients.Length; i++)
        {
            Debug.Log(_ingredients[i].ID);
        }
        OnInventoryUpdated?.Invoke();
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
        return RoomInfoManager.Instance.CurrentRoomInfo.KnownIngredients.Contains(ingredientID);
    }

    public bool IsKnownRecipe(int recipeID)
    {
        return RoomRecipeStateManager.Instance.IsUnlocked(recipeID);
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

