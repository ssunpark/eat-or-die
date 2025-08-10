using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeIngredient : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private readonly Dictionary<int, UI_IngredientButton> _ingredientDataList = new();
    private bool _isInitialized = false;

    private void OnEnable()
    {
        RoomRecipeStateManager.OnIngredientUnlocked += HandleIngredientUnlocked;
    }

    private void OnDisable()
    {
        RoomRecipeStateManager.OnIngredientUnlocked -= HandleIngredientUnlocked;
    }

    public void Init()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        var ingredientIdList = ItemManager.Instance.GetFoodIngredientList();
        _ingredientDataList.Clear();

        foreach (var ingredientId in ingredientIdList)
        {
            if (ingredientId == null)
            {
                continue;
            }
            
            GameObject buttonObj = Instantiate(ButtonPrefab, Container.transform);
            var button = buttonObj.GetComponent<UI_IngredientButton>();
            button.Refresh(ingredientId);
            buttonObj.SetActive(true);
            _ingredientDataList[ingredientId.ID] = button;
        }
    }

    // public void ShowFilteredIngredients()
    // {
    //     foreach (var button in _ingredientDataList.Values)
    //     {
    //         button.gameObject.SetActive(true);
    //         button.LockButton();
    //     }
    //
    //     // 현재 인벤토리에 있는 재료들만 활성화
    //     var ingredientList = RecipePanelUIManager.Instance.Ingredients;
    //     foreach (var item in ingredientList)
    //     {
    //         if (_ingredientDataList.TryGetValue(item.ID, out var button))
    //         {
    //             button.gameObject.SetActive(true);
    //             button.UnlockButton();
    //         }
    //     }
    // }

    private void HandleIngredientUnlocked(int unlockedIngredientID)
    {
        RefreshIngredientButtons();
    }

    private void RefreshIngredientButtons()
    {
        // 모두 비활성화
        foreach (var button in _ingredientDataList.Values)
        {
            button.Refresh(button.GetIngredient());
        }
    }

}