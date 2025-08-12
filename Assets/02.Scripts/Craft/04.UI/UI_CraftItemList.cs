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
        // CraftRecipeManager.Instance.OnDataLoaded += CreateButtons;
        InventoryManager.Instance.OnInventoryUpdated += RefrehCraftRecipButtons;
    }

    private void OnDisable()
    {
        // CraftRecipeManager.Instance.OnDataLoaded -= CreateButtons;
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated -= RefrehCraftRecipButtons;
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

    // private void CreateAllButtons(int minID, int maxID, GameObject container)
    // {
    //     List<CraftRecipe> craftRecipes = CraftRecipeManager.Instance.GetAll();
    //     foreach (var craftRecipe in craftRecipes)
    //     {
    //         if (craftRecipe.CraftResultID < minID || craftRecipe.CraftResultID >= maxID) continue;
    //         ItemProfile itemProfile = ItemManager.Instance.GetItem(craftRecipe.CraftResultID);
    //         if (itemProfile == null || itemProfile.ItemDefinition == null) continue;
    //         
    //         GameObject btn = Instantiate(CraftItemPrefab, container.transform);
    //         UI_CraftItemButton craftItemButtons = btn.GetComponent<UI_CraftItemButton>();
    //         craftItemButtons.Refresh(craftRecipe);
    //         btn.SetActive(false);
    //         _craftItemButtons.Add(craftItemButtons);
    //     }
    // }

    public void RefrehCraftRecipButtons()
    {
        Debug.Log("RefrehCraftRecipButtons 메서드 진입!");
        foreach (var button in _craftItemButtonList)
        {
            Debug.Log("foreach문 진입");
            button.CanCraft();
            button.gameObject.SetActive(true);
        }
    }
}
