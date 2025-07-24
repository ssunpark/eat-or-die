using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeIngredient : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;
    
    private Dictionary<int, UI_IngredientButton> _ingredientButtonDict = new Dictionary<int, UI_IngredientButton>();

    private bool _isInitialized = false;

    private void OnEnable()
    {
        RecipePanelManager.Instance.OnInventoryUpdated += RefreshIngredientButtons;
    }

    public void Init()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        
        List<int> ingredientIdList = ItemManager.Instance.GetFoodIngredientList();
        _ingredientButtonDict.Clear();
        

        foreach (int id in ingredientIdList)
        {
            AItemInfo itemInfo = ItemManager.Instance.GetItem(id);
            if (itemInfo == null) continue;

            GameObject buttonObj = Instantiate(ButtonPrefab, Container.transform);
            var button = buttonObj.GetComponent<UI_IngredientButton>();
            button.Refresh(itemInfo.ItemData); // <- AItemInfo 넘김
            buttonObj.SetActive(false);

            _ingredientButtonDict[itemInfo.ItemData.ID] = button;
        }
    }

    private void RefreshIngredientButtons()
    {
        // 모두 비활성화
        foreach (var button in _ingredientButtonDict.Values)
        {
            button.gameObject.SetActive(false);
        }

        // 현재 인벤토리에 있는 재료들만 활성화
        var ingredientList = RecipePanelManager.Instance.Ingredients;
        if (ingredientList != null && ingredientList.Length > 0)
        {
            Debug.Log(ingredientList[0].ID);
        }

        foreach (var item in ingredientList)
        {
            if (_ingredientButtonDict.TryGetValue(item.ID, out var button))
            {
                button.gameObject.SetActive(true);
            }
        }
    }
}