using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// 수현
public class UI_CraftItemList : MonoBehaviour
{
    public GameObject Container;
    public GameObject CraftItemPrefab;
    
    private readonly List<UI_CraftItemButton> _craftItemButtons = new();

    private void OnEnable()
    {
        CraftRecipeManager.Instance.OnDataLoaded += CreateButtons;
        InventoryManager.Instance.OnInventoryUpdated += RefreshAllButtons;
    }

    private void OnDisable()
    {
        CraftRecipeManager.Instance.OnDataLoaded -= CreateButtons;
        InventoryManager.Instance.OnInventoryUpdated -= RefreshAllButtons;
    }

    private void CreateButtons()
    {
        ClearButtons();

        List<CraftRecipe> craftRecipes = CraftRecipeManager.Instance.GetAll();
        foreach (var craftRecipe in craftRecipes)
        {
            Debug.Log("UI_CraftItemList.foreach문 진입");
            AItemInfo itemInfo = ItemManager.Instance.GetItem(craftRecipe.CraftResultID);
            if (itemInfo == null || itemInfo.ItemData == null) continue;
            
            GameObject btn = Instantiate(CraftItemPrefab, Container.transform);
            Debug.Log("버튼 생성!!!!!!!");
            UI_CraftItemButton craftItemButtons = btn.GetComponent<UI_CraftItemButton>();
            craftItemButtons.Refresh(craftRecipe, itemInfo);
            btn.SetActive(false);
            _craftItemButtons.Add(craftItemButtons);
        }

        RefreshAllButtons();
    }

    private void RefreshAllButtons()
    {
        foreach (var button in _craftItemButtons)
        {
            button.CanInteractable();
            button.gameObject.SetActive(true);
        }
    }

    private void ClearButtons()
    {
        foreach (var buttons in _craftItemButtons)
        {
            buttons.gameObject.SetActive(false);
        }
        _craftItemButtons.Clear();
    }
    
}
