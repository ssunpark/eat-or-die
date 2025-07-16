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
    }
}
