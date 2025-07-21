using System;
using System.Linq;
using UnityEngine;

public class RecipePanelManager : BehaviourSingleton<RecipePanelManager>
{
    private ItemStack[] _ingredients ;
    public ItemStack[] Ingredients => _ingredients;
    public event Action OnInventoryUpdated;
    private void OnEnable()
    {
        InventoryManager.Instance.OnInventoryUpdated += UpdateIngredients;
    }

    // InventoryManager에 등록된 재료를 조건(ID)으로 필터
    // 디버그로 확인하고 이벤트 호출
    public void UpdateIngredients()
    {
        _ingredients = InventoryManager.Instance.Inventory.SlotList
            .Where(slot => slot.ItemStack != null && slot.ItemStack.ID >= 200000 && slot.ItemStack.ID < 300000)
            .Select(slot => slot.ItemStack)
            .ToArray();
        for (int i = 0; i < _ingredients.Length; i++)
        {
            Debug.Log(_ingredients[i].ID);
        }
        OnInventoryUpdated?.Invoke();
    }
}

