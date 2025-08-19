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
        200028 // 드래곤 고기
        // 여기에 더 추가...
    };

    private void Start()
    {
        RoomRecipeStateManager.Instance.OnIngredientUnlocked += HandleIngredientUnlocked;
    }

// private void OnEnable()
//     {
//     }
//
//     private void OnDisable()
//     {
//         if (RoomRecipeStateManager.Instance != null)
//         {
//             RoomRecipeStateManager.Instance.OnIngredientUnlocked -= HandleIngredientUnlocked;
//         }
//     }
    
    public void Init()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        var allIngredients = ItemManager.Instance.GetFoodIngredientList();
        _ingredientDataList.Clear();

        foreach (var ingredientId in allIngredients)
        {
            if (ingredientId == null)
            {
                continue;
            }

            // 제외 목록에 포함된 ID라면 버튼을 만들지 않고 건너뜀
            if (_excludedIngredientIDs.Contains(ingredientId.ID))
            {
                continue;
            }

            var buttonObj = Instantiate(ButtonPrefab, Container.transform);
            var button = buttonObj.GetComponent<UI_IngredientButton>();
            button.Refresh(ingredientId);
            buttonObj.SetActive(true);
            _ingredientDataList[ingredientId.ID] = button;
        }
    }

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