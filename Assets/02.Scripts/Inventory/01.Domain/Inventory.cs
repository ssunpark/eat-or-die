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
    
    public ItemInstance GetItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return null;
        }
        
        return SlotList[slotIndex].ItemInstance;
    }

    public ItemInstance PopItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return null;
        }
        
        ItemInstance slotItemInstance = GetItemInSlot(slotIndex);
        SlotList[slotIndex].RemoveItem();
        return slotItemInstance;
    }
    
    public ItemInstance PopSingleItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return null;
        }

        Slot targetSlot = SlotList[slotIndex];
        
        if (targetSlot.IsEmpty) return null;
        
        ItemInstance itemInstance = targetSlot.ItemInstance;
        
        if (itemInstance.Quantity > 1)
        {
            itemInstance.SetQuantity(itemInstance.Quantity - 1);
            return new ItemInstance(itemInstance.ItemProfile, 1);
        }
        else
        {
            targetSlot.RemoveItem();
            return itemInstance;
        }
    }
    
    public ItemInstance PutItemInSlot(int slotIndex, ItemInstance itemInstance)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return itemInstance;
        }

        Slot targetSlot = SlotList[slotIndex];
        
        if (targetSlot.IsEmpty)
        {
            targetSlot.AddItem(itemInstance);
            return null;
        }
        
        if (targetSlot.ItemInstance.ID == itemInstance.ID)
        {
            if (targetSlot.ItemInstance.Quantity + itemInstance.Quantity > targetSlot.ItemInstance.MaxQuantity)
            {
                int excessQuantity = targetSlot.ItemInstance.Quantity + itemInstance.Quantity - targetSlot.ItemInstance.MaxQuantity;
                targetSlot.ItemInstance.SetQuantity(targetSlot.ItemInstance.MaxQuantity);
                itemInstance.SetQuantity(excessQuantity);
                return itemInstance;
            }
            else
            {
                targetSlot.ItemInstance.TryAdd(itemInstance.Quantity);
                return null;
            }
        }
        else
        {
            ItemInstance temp = targetSlot.ItemInstance;
            targetSlot.AddItem(itemInstance);
            return temp;
        }
    }

    public ItemInstance PickItemFromGround(ItemInstance itemInstance)
    {
        foreach (Slot slot in SlotList)
        {
            if (slot.IsEmpty)
            {
                continue;
            }
            if (slot.ItemInstance.ID == itemInstance.ID)
            {
                if (slot.ItemInstance.Quantity + itemInstance.Quantity > slot.ItemInstance.MaxQuantity)
                {
                    int excessQuantity = slot.ItemInstance.Quantity + itemInstance.Quantity - slot.ItemInstance.MaxQuantity;
                    slot.ItemInstance.SetQuantity(slot.ItemInstance.MaxQuantity);
                    itemInstance.SetQuantity(excessQuantity);
                }
                else
                {
                    slot.ItemInstance.TryAdd(itemInstance.Quantity);
                    return null;
                }
            }
        }
        
        foreach (Slot slot in SlotList)
        {
            if (slot.IsEmpty)
            {
                slot.AddItem(itemInstance);
                return null;
            }
        }

        return itemInstance;
    }

    public bool HaveItem(int itemID)
    {
        return GetItemCount(itemID) > 0;
    }
    
    public int GetItemCount(int itemID)
    {
        int count = 0;
        foreach (Slot slot in SlotList)
        {
            if (!slot.IsEmpty && slot.ItemInstance.ID == itemID)
            {
                count += slot.ItemInstance.Quantity;
            }
        }
        return count;
    }
    
    // id로 조회해서 원하는 count만큼 개수 감소
    public bool TryConsumeItem(int itemID, int amount)
    {
        int currentCount = GetItemCount(itemID);
        if (currentCount < amount)
        {
            return false;
        }

        int remaining = amount;

        foreach (Slot slot in SlotList)
        {
            if (slot.IsEmpty || slot.ItemInstance.ID != itemID)
            {
                continue;
            }
            if (slot.ItemInstance.Quantity > remaining)
            {
                slot.ItemInstance.TryRemove(remaining);
                return true;
            }
            else
            {
                remaining -= slot.ItemInstance.Quantity;
                slot.RemoveItem();
            }
            
            if (remaining <= 0)
            {
                return true;
            }
        }
        return true;
    }
}
