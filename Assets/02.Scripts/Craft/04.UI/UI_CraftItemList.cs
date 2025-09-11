using System.Collections.Generic;
using UnityEngine;

public class UI_CraftItemList : MonoBehaviour
{
    public GameObject Container;
    public GameObject CraftItemPrefab;

    private List<CraftRecipe> _craftRecipeDataList = new();
    private readonly List<UI_CraftItemButton> _craftItemButtonList = new();
    private UI_CraftItemButton _currentSelectedButton;

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
            craftRecipeButton.Init(craftRecipe);
            _craftItemButtonList.Add(craftRecipeButton);
        }
    }
    
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
    
    public void SetSelectedItem(int craftRecipeID)
    {
        if (_currentSelectedButton != null)
        {
            _currentSelectedButton.SetSelected(false);
        }
        
        _currentSelectedButton = _craftItemButtonList.Find(btn => btn.CraftRecipeID == craftRecipeID);
        if (_currentSelectedButton != null)
        {
            _currentSelectedButton.SetSelected(true);
        }
    }
}
