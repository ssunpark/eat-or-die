using System.Collections.Generic;
using UnityEngine;

// 수현
public class UI_CraftItemList : MonoBehaviour
{
    public GameObject Container;
    public GameObject CraftItemPrefab;

    private List<CraftRecipe> _craftRecipeDataList = new();
    private readonly List<UI_CraftItemButton> _craftItemButtonList = new();

    private void OnEnable()
    {
        RefrehCraftRecipButtons();
        UnifiedInventoryManager.Instance.OnPossessionUpdated += RefrehCraftRecipButtons;
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            UnifiedInventoryManager.Instance.OnPossessionUpdated -= RefrehCraftRecipButtons;
        }
    }

    public void Init()
    {
        _craftRecipeDataList = CraftRecipeManager.Instance.CraftRecipeList;

        foreach (var craftRecipe in _craftRecipeDataList)
        {
            var buttonObj = Instantiate(CraftItemPrefab, Container.transform);
            var craftRecipeButton = buttonObj.GetComponent<UI_CraftItemButton>();
            craftRecipeButton.Init(craftRecipe); // 처음엔 craft가 가능한지에 대해서 리프레시, 요리솥 SetDetail 리프레시
            _craftItemButtonList.Add(craftRecipeButton);
        }
    }

    // 카테고리 분리해서 보여주기용
    public void ShowFilterCraftItems(List<CraftRecipe> craftRecips)
    {
        foreach (var button in _craftItemButtonList)
        {
            button.gameObject.SetActive(false);
        }

        foreach (var craftRecipe in craftRecips)
        {
            var match = _craftItemButtonList.Find(btn => btn.CraftRecipeID == craftRecipe.CraftResultID);
            if (match != null)
            {
                match.gameObject.SetActive(true);
            }
        }
    }

    public void RefrehCraftRecipButtons()
    {
        foreach (var button in _craftItemButtonList)
        {
            button.CanCraft();
        }
    }
}
