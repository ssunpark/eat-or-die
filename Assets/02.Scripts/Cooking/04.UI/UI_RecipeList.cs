using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private List<Recipe> _recipeCsvDataList = new List<Recipe>();
    private List<UI_RecipeButton> _recipeButtonList = new List<UI_RecipeButton>();

    private void OnEnable()
    {
        RoomRecipeStateManager.OnRecipeUnlocked += HandleRecipeUnlocked;
        CookingManager.OnItemAdded += RefreshRecipeButtons;
    }
    
    private void OnDisable()
    {
        RoomRecipeStateManager.OnRecipeUnlocked -= HandleRecipeUnlocked;
        if (CookingManager.Instance != null)
        {
            CookingManager.OnItemAdded -= RefreshRecipeButtons;
        }
    }

    // 최초 1회만 호출해서 버튼 생성
    public void Init()
    {
        _recipeCsvDataList = RecipeManager.Instance.RecipeList;

        foreach (var recipe in _recipeCsvDataList)
        {
            var buttonObj = Instantiate(ButtonPrefab, Container.transform);
            var recipeButton = buttonObj.GetComponent<UI_RecipeButton>();
            recipeButton.Refresh(recipe);
            buttonObj.SetActive(false); // 처음엔 꺼둠
            _recipeButtonList.Add(recipeButton);
        }
    }


    public void ShowFilteredRecipes(List<Recipe> recipes)
    {
        // 전부 비활성화
        foreach (var button in _recipeButtonList)
        {
            button.gameObject.SetActive(false);
        }

        // 조건에 맞는 것만 활성화
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
        // 이벤트가 오면, 자신이 가진 버튼들을 새로고침합니다.
        RefreshRecipeButtons();
    }

    public void RefreshRecipeButtons()
    {
        Debug.Log("RefreshRecipeButtons");
        foreach (var button in _recipeButtonList)
        {
            button.Refresh(button.GetRecipe());
        }

        RecipePanelUIManager.Instance.UpdateRecipes();
    }
}