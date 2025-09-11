using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeIngredient : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private readonly Dictionary<int, UI_IngredientButton> _ingredientDataList = new();
    private bool _isInitialized = false;

    // 제외하고 싶은 재료의 ID 목록
    private readonly HashSet<int> _excludedIngredientIDs = new()
    {
        200012, // 썩은 작물
        200013, // 강철
        200028, // 드래곤 고기
        600004, // 덤불칼
        600005, // 환각버섯 도끼
        600006 // 대지 뿌리봉
    };

    private void Start()
    {
        RoomRecipeStateManager.Instance.OnIngredientUnlocked += HandleIngredientUnlocked;
    }
    
    public void PopulateIngredients(ERecipeCategory category)
    {
        foreach (Transform child in Container.transform)
        {
            Destroy(child.gameObject);
        }
        _ingredientDataList.Clear();
        
        var ingredients = ItemManager.Instance.GetIngredientsByCategory(category);

        foreach (var ingredient in ingredients)
        {
            if (ingredient == null || _excludedIngredientIDs.Contains(ingredient.ID))
            {
                continue;
            }

            var buttonObj = Instantiate(ButtonPrefab, Container.transform);
            var button = buttonObj.GetComponent<UI_IngredientButton>();
            button.Refresh(ingredient);
            buttonObj.SetActive(true);
            _ingredientDataList[ingredient.ID] = button;
        }
    }


    private void HandleIngredientUnlocked(int unlockedIngredientID)
    {
        RefreshIngredientButtons();
    }

    private void RefreshIngredientButtons()
    {
        foreach (var button in _ingredientDataList.Values)
        {
            button.Refresh(button.GetIngredient());
        }
    }

}