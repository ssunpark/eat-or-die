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

