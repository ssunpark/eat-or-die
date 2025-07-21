using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeIngredient : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private List<IngredientCSVData> _ingredientCsvDataList;
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

        _ingredientCsvDataList = FoodCSVDataManager.Instance.IngredientCsvDataList;
        _ingredientButtonDict.Clear();
        

        foreach (var ingredientData in _ingredientCsvDataList)
        {
            GameObject buttonObj = Instantiate(ButtonPrefab, Container.transform);
            var button = buttonObj.GetComponent<UI_IngredientButton>();
            button.Refresh(ingredientData);
            buttonObj.SetActive(false); // 처음엔 다 꺼두기

            _ingredientButtonDict[ingredientData.ID] = button;
        }

        // 인벤토리 이벤트 구독 (한 번만)
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