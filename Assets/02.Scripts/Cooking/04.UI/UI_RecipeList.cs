using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private List<RecipeCSVData> _recipeCsvDataList = new List<RecipeCSVData>();
    private List<GameObject> _recipeButtonList =  new List<GameObject>();

    // CSV 불러와서 버튼 생성 - 초기화
    public void Init()
    {
        _recipeCsvDataList = FoodCSVDataManager.Instance.RecipeCSVDataList;
        ShowAllRecipes();
    }

    public void ShowAllRecipes()
    {
        ClearAllButtons();

        foreach (var recipe in _recipeCsvDataList)
        {
            CreateRecipeButton(recipe);
        }
    }
    
    public void ShowFilteredRecipes(List<RecipeCSVData> recipes)
    {
        ClearAllButtons();

        foreach (var recipe in recipes)
        {
            CreateRecipeButton(recipe);
        }
    }

    private void CreateRecipeButton(RecipeCSVData recipe)
    {
        var buttonObj = Instantiate(ButtonPrefab, Container.transform);
        var recipeButton = buttonObj.GetComponent<UI_RecipeButton>();
        recipeButton.Refresh(recipe);
        _recipeButtonList.Add(buttonObj);
        buttonObj.SetActive(true);
    }

    private void ClearAllButtons()
    {
        foreach (var button in _recipeButtonList)
        {
            button.SetActive(false);
        }
        _recipeButtonList.Clear();
    }
}