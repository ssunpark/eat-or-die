using System.Linq;
using UnityEngine;

public class CraftRecipeUIManager : BehaviourSingleton<CraftRecipeUIManager>
{
    public UI_CraftItemList CraftItemListUI;

    public void UpdateAllCraftItems()
    {
        Debug.Log("UpdateAllCraftItems");
        var allCraftRecipes = CraftRecipeManager.Instance.CraftRecipeList;
        CraftItemListUI.ShowFilterCraftItems(allCraftRecipes);
    }

    public void UpdateToolItems()
    {
        Debug.Log("UpdateToolItems");
        var filteredToolRecipes = CraftRecipeManager.Instance.CraftRecipeList
            .Where(recipe => recipe.CraftResultID >= 400000 && recipe.CraftResultID < 600000)
            .ToList();
        CraftItemListUI.ShowFilterCraftItems(filteredToolRecipes);
    }

    public void UpdateWeaponItems()
    {
        Debug.Log("UpdateWeaponItems");
        var filteredWeaponRecipes = CraftRecipeManager.Instance.CraftRecipeList
            .Where(recipe => recipe.CraftResultID >= 600000 && recipe.CraftResultID < 700000)
            .ToList();
        CraftItemListUI.ShowFilterCraftItems(filteredWeaponRecipes);
    }

    public void UpdateEquipItems()
    {
        Debug.Log("UpdateEquipItems");
        var filteredEquipRecipes = CraftRecipeManager.Instance.CraftRecipeList
            .Where(recipe => recipe.CraftResultID >= 700000 && recipe.CraftResultID < 800000)
            .ToList();

        CraftItemListUI.ShowFilterCraftItems(filteredEquipRecipes);
    }
}