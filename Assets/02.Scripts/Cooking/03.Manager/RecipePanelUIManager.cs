using System;
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
        _ingredients = InventoryManager.Instance.Inventory.SlotList
            .Where(slot => slot.Item != null && slot.Item.ID >= 200000 && slot.Item.ID < 300000)
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
            .Where(recipe => recipe.Ingredient1ID == ingredientID || recipe.Ingredient2ID == ingredientID)
            .ToList();

        Debug.Log($"[RecipePanel] Found {filteredRecipes.Count} recipes with Ingredient ID {ingredientID}");
        RecipeListUI.ShowFilteredRecipes(filteredRecipes);
    }
}

