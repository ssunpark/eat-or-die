using System;
using UnityEngine;

public class HandEntity : BehaviourSingleton<HandEntity>
{
    public ItemStack ItemStack;
    public bool IsHandEmpty => ItemStack == null;
    
    public Action OnItemPickedUp;
    
    public void PickUpItem(ItemStack itemStack)
    {
        ItemStack = itemStack;
        OnItemPickedUp?.Invoke();
    }

    public bool TryAddItem(ItemStack itemStack)
    {
        if (itemStack.ID != ItemStack.ID) return false;

        if (!ItemStack.TryAdd(itemStack.Quantity)) return false;
        
        OnItemPickedUp?.Invoke();
        return true;
    }
    
    public void DropItem()
    {
        ItemStack = null;
        OnItemPickedUp?.Invoke();
    }
}
