using System;
using DarkTonic.MasterAudio;

public class HandEntity : BehaviourSingleton<HandEntity>
{
    public ItemInstance ItemInstance;
    public bool IsHandEmpty => ItemInstance == null;
    
    public event Action OnItemPickedUp;
    public event Action OnItemDropped;
    
    public void PickUpItem(ItemInstance itemInstance)
    {
        ItemInstance = itemInstance;
        OnItemPickedUp?.Invoke();
        MasterAudio.PlaySound("ButtonClick");
    }

    public ItemInstance GetItem()
    {
        return ItemInstance;
    }

    public bool TryAddItem(ItemInstance itemInstance)
    {
        if (itemInstance == null) return false;
        
        if (IsHandEmpty) 
        {
            PickUpItem(itemInstance);
            return true;
        }
        
        if (itemInstance.ID != ItemInstance.ID) return false;

        if (!ItemInstance.TryAdd(itemInstance.Quantity)) return false;
        
        OnItemPickedUp?.Invoke();
        MasterAudio.PlaySound("ButtonClick");
        return true;
    }
    
    public void DropItem()
    {
        ItemInstance = null;
        OnItemPickedUp?.Invoke();
        OnItemDropped?.Invoke();
    }
}
