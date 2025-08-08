using System;
using UnityEngine;

public class HandEntity : BehaviourSingleton<HandEntity>
{
    public ItemInstance ItemInstance;
    public bool IsHandEmpty => ItemInstance == null;
    
    public Action OnItemPickedUp;
    
    public void PickUpItem(ItemInstance itemInstance)
    {
        if (itemInstance != null)
        {
            Debug.Log($"Item picked up: {itemInstance.ID}");
        }
        ItemInstance = itemInstance;
        OnItemPickedUp?.Invoke();
    }

    public ItemInstance GetItem()
    {
        return ItemInstance;
    }

    public bool TryAddItem(ItemInstance itemInstance)
    {
        if (itemInstance.ID != ItemInstance.ID) return false;

        if (!ItemInstance.TryAdd(itemInstance.Quantity)) return false;
        
        OnItemPickedUp?.Invoke();
        return true;
    }
    
    public void DropItem()
    {
        ItemInstance = null;
        OnItemPickedUp?.Invoke();
    }
}
