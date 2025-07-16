using UnityEngine;

public class HandEntity : BehaviourSingleton<HandEntity>
{
    private ItemStack _itemStack;
    public bool IsHandEmpty => _itemStack == null;
    
    public void PickUpItem(ItemStack itemStack)
    {
        _itemStack = itemStack;
    }
}
