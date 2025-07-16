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
    
    public void DropItem()
    {
        ItemStack = null;
        OnItemPickedUp?.Invoke();
    }
}
