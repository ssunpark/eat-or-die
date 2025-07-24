using System;
using UnityEngine;

public class HandEntity : BehaviourSingleton<HandEntity>
{
    public Item Item;
    public bool IsHandEmpty => Item == null;
    
    public Action OnItemPickedUp;
    
    public void PickUpItem(Item item)
    {
        if (item != null)
        {
            Debug.Log($"Item picked up: {item.ID}");
        }
        Item = item;
        OnItemPickedUp?.Invoke();
    }

    public bool TryAddItem(Item item)
    {
        if (item.ID != Item.ID) return false;

        if (!Item.TryAdd(item.Quantity)) return false;
        
        OnItemPickedUp?.Invoke();
        return true;
    }
    
    public void DropItem()
    {
        Item = null;
        OnItemPickedUp?.Invoke();
    }
}
