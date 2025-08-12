using System.Linq;

public class CraftRecipeUIManager : BehaviourSingleton<CraftRecipeUIManager>
{
    public UI_CraftItemList CraftItemListUI;

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
}