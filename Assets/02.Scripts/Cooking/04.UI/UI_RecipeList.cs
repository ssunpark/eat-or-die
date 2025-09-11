using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private List<Recipe> _recipeCsvDataList = new List<Recipe>();
    private List<UI_RecipeButton> _recipeButtonList = new List<UI_RecipeButton>();

    private void Start()
    {
        RoomRecipeStateManager.Instance.OnRecipeUnlocked += HandleRecipeUnlocked;
        UnifiedInventoryManager.Instance.OnPossessionUpdated += RefreshRecipeButtons;
        RefreshRecipeButtons();
    }
    
    public void Init()
    {
        _recipeCsvDataList = RecipeManager.Instance.RecipeList;

        foreach (var recipe in _recipeCsvDataList)
        {
            var buttonObj = Instantiate(ButtonPrefab, Container.transform);
            var recipeButton = buttonObj.GetComponent<UI_RecipeButton>();
            recipeButton.Refresh(recipe);
            _recipeButtonList.Add(recipeButton);
        }
    }


    public void ShowFilteredRecipes(List<Recipe> recipes)
    {
        foreach (var button in _recipeButtonList)
        {
            button.gameObject.SetActive(false);
        }

        foreach (var recipe in recipes)
        {
            var match = _recipeButtonList.Find(btn => btn.RecipeID == recipe.ID);
            if (match != null)
            {
                match.gameObject.SetActive(true);
            }
        }
    }

    private void HandleRecipeUnlocked(Recipe unlockedRecipe)
    {
        RefreshRecipeButtons();
    }

    public void RefreshRecipeButtons()
    {
        foreach (var button in _recipeButtonList)
        {
            button.Refresh(button.GetRecipe());
        }

        RecipePanelUIManager.Instance.UpdateRecipes();
    }
}