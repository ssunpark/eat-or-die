using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// 수현
public class UI_CraftItemList : MonoBehaviour
{
    public GameObject Container1;
    public GameObject Container2;
    public GameObject Container3;
    public GameObject CraftItemPrefab;
    
    private readonly List<UI_CraftItemButton> _craftItemButtons = new();
    private bool isInitialized = false;

    private void OnEnable()
    {
        if (!isInitialized)
        {
            InitButtons();
            isInitialized = true;
        }
        RefreshAllButtons();
        // CraftRecipeManager.Instance.OnDataLoaded += CreateButtons;
        InventoryManager.Instance.OnInventoryUpdated += RefreshAllButtons;
    }

    private void OnDisable()
    {
        // CraftRecipeManager.Instance.OnDataLoaded -= CreateButtons;
        InventoryManager.Instance.OnInventoryUpdated -= RefreshAllButtons;
    }

    private void InitButtons()
    {
        CreateAllButtons(400000, 600000, Container1);
        CreateAllButtons(600000, 700000, Container2);
        CreateAllButtons(700000, 800000, Container3);
    }

    private void CreateAllButtons(int minID, int maxID, GameObject container)
    {
        List<CraftRecipe> craftRecipes = CraftRecipeManager.Instance.GetAll();
        foreach (var craftRecipe in craftRecipes)
        {
            if (craftRecipe.CraftResultID < minID || craftRecipe.CraftResultID >= maxID) continue;
            AItemInfo itemInfo = ItemManager.Instance.GetItem(craftRecipe.CraftResultID);
            if (itemInfo == null || itemInfo.ItemData == null) continue;
            
            GameObject btn = Instantiate(CraftItemPrefab, container.transform);
            Debug.Log("버튼 생성!!!!!");
            UI_CraftItemButton craftItemButtons = btn.GetComponent<UI_CraftItemButton>();
            craftItemButtons.Refresh(craftRecipe, itemInfo);
            btn.SetActive(false);
            _craftItemButtons.Add(craftItemButtons);
        }
    }

    private void RefreshAllButtons()
    {
        foreach (var button in _craftItemButtons)
        {
            button.CanInteractable();
            button.gameObject.SetActive(true);
        }
    }
}
