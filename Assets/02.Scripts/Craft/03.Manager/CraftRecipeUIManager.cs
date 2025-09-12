using System.Linq;
using UnityEngine;

public class CraftRecipeUIManager : BehaviourSingleton<CraftRecipeUIManager>
{
    [SerializeField] private UI_CraftItemList CraftItemListUI;
    [SerializeField] private UI_CraftDetailPanel CraftDetailPanelUI;

    private void Start()
    {
        CraftDetailPanelUI.CreateIngredientButtons();
    }
    public void UpdateAllCraftItems()
    {
        var allCraftRecipes = CraftRecipeManager.Instance.CraftRecipeList;
        CraftItemListUI.ShowFilterCraftItems(allCraftRecipes);
    }

    public void UpdateToolItems()
    {
        var filteredToolRecipes = CraftRecipeManager.Instance.CraftRecipeList
            .Where(recipe => recipe.CraftResultID >= 400000 && recipe.CraftResultID < 600000)
            .ToList();
        CraftItemListUI.ShowFilterCraftItems(filteredToolRecipes);
    }

    public void UpdateWeaponItems()
    {
        var filteredWeaponRecipes = CraftRecipeManager.Instance.CraftRecipeList
            .Where(recipe => recipe.CraftResultID >= 600000 && recipe.CraftResultID < 700000)
            .ToList();
        CraftItemListUI.ShowFilterCraftItems(filteredWeaponRecipes);
    }

    public void UpdateEquipItems()
    {
        var filteredEquipRecipes = CraftRecipeManager.Instance.CraftRecipeList
            .Where(recipe => recipe.CraftResultID >= 700000 && recipe.CraftResultID < 800000)
            .ToList();

        CraftItemListUI.ShowFilterCraftItems(filteredEquipRecipes);
    }

    public void SelectCraftItem(CraftRecipe craftRecipe)
    {
        CraftDetailPanelUI.UpdateDetails(craftRecipe);
        CraftDetailPanelUI.RefreshCraftCount();
        CraftItemListUI.SetSelectedItem(craftRecipe.CraftResultID);
    }
}