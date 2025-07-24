using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private List<Recipe> _recipeCsvDataList = new List<Recipe>();
    private List<UI_RecipeButton> _recipeButtonList = new List<UI_RecipeButton>();
    
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

    public void ShowAllRecipes()
    {
        foreach (var button in _recipeButtonList)
        {
            button.gameObject.SetActive(true);
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

    // 전체 숨기기
    public void HideAll()
    {
        foreach (var button in _recipeButtonList)
        {
            button.gameObject.SetActive(false);
        }
    }
    
    // 해금 메서드
    public void UnlockRecipe(int resultItemId)
    {
        Debug.Log("UI_RecipeList::UnlockRecipe");
        var recipe = _recipeButtonList.Find(btn => btn.ResultItemID == resultItemId);
        if (recipe != null)
        {
            Debug.Log("UI_RecipeList recipe 널 체크");
            recipe.UnlockButton();
        }
    }
}