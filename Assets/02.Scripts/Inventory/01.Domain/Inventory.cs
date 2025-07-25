using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private int _inventorySize;

    public List<Slot> SlotList = new List<Slot>();

    public Inventory(int inventorySize)
    {
        _inventorySize = inventorySize;
        for (int i = 0; i < _inventorySize; i++)
        {
            SlotList.Add(new Slot());
        }
    }
    
    public Item GetItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return null;
        }
        
        return SlotList[slotIndex].Item;
    }

    public Item PopItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return null;
        }
        
        Item slotItem = GetItemInSlot(slotIndex);
        SlotList[slotIndex].RemoveItem();
        return slotItem;
    }
    
    public Item PopSingleItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return null;
        }

        Slot targetSlot = SlotList[slotIndex];
        
        if (targetSlot.IsEmpty) return null;
        
        Item item = targetSlot.Item;
        
        if (item.Quantity > 1)
        {
            item.SetQuantity(item.Quantity - 1);
            return new Item(item.ItemInfo, item.MaxQuantity, 1);
        }
        else
        {
            targetSlot.RemoveItem();
            return item;
        }
    }
    
    public Item PutItemInSlot(int slotIndex, Item item)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return item;
        }

        Slot targetSlot = SlotList[slotIndex];
        
        if (targetSlot.IsEmpty)
        {
            targetSlot.AddItem(item);
            return null;
        }
        
        if (targetSlot.Item.ID == item.ID)
        {
            if (targetSlot.Item.Quantity + item.Quantity > targetSlot.Item.MaxQuantity)
            {
                int excessQuantity = targetSlot.Item.Quantity + item.Quantity - targetSlot.Item.MaxQuantity;
                targetSlot.Item.SetQuantity(targetSlot.Item.MaxQuantity);
                item.SetQuantity(excessQuantity);
                return item;
            }
            else
            {
                targetSlot.Item.TryAdd(item.Quantity);
                return null;
            }
        }
        else
        {
            Item temp = targetSlot.Item;
            targetSlot.AddItem(item);
            return temp;
        }
    }

    public Item PickItemFromGround(Item item)
    {
        foreach (Slot slot in SlotList)
        {
            if (slot.IsEmpty)
            {
                continue;
            }
            if (slot.Item.ID == item.ID)
            {
                if (slot.Item.Quantity + item.Quantity > slot.Item.MaxQuantity)
                {
                    int excessQuantity = slot.Item.Quantity + item.Quantity - slot.Item.MaxQuantity;
                    slot.Item.SetQuantity(slot.Item.MaxQuantity);
                    item.SetQuantity(excessQuantity);
                }
                else
                {
                    slot.Item.TryAdd(item.Quantity);
                    return null;
                }
            }
        }
        
        foreach (Slot slot in SlotList)
        {
            if (slot.IsEmpty)
            {
                slot.AddItem(item);
                return null;
            }
        }

        return item;
    }
}
