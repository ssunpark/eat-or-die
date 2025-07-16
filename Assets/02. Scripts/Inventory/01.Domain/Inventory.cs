using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private int _inventorySize;
    
    public List<Slot> SlotList { get; private set; }

    public Inventory(int inventorySize)
    {
        _inventorySize = inventorySize;
        SlotList = new List<Slot>(_inventorySize);
    }

    public ItemStack PopItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return null;
        }
        
        ItemStack slotItem =  SlotList[slotIndex].ItemStack;
        SlotList.RemoveAt(slotIndex);
        return slotItem;
    }
    
    public ItemStack PutItemInSlot(int slotIndex, ItemStack itemStack)
    {
        if (slotIndex < 0 || slotIndex >= SlotList.Count)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return itemStack;
        }

        Slot targetSlot = SlotList[slotIndex];
        
        if (targetSlot.IsEmpty)
        {
            targetSlot.AddItem(itemStack);
            return null;
        }
        
        if (targetSlot.ItemStack.ID == itemStack.ID)
        {
            if (targetSlot.ItemStack.Quantity + itemStack.Quantity > targetSlot.ItemStack.MaxQuantity)
            {
                int excessQuantity = targetSlot.ItemStack.Quantity + itemStack.Quantity - targetSlot.ItemStack.MaxQuantity;
                targetSlot.ItemStack.SetQuantity(targetSlot.ItemStack.MaxQuantity);
                itemStack.SetQuantity(excessQuantity);
                return itemStack;
            }
            else
            {
                targetSlot.ItemStack.TryAdd(itemStack.Quantity);
                return null;
            }
        }
        else
        {
            ItemStack temp = targetSlot.ItemStack;
            targetSlot.AddItem(itemStack);
            return temp;
        }
    }

    public ItemStack PickItemFromGround(ItemStack itemStack)
    {
        foreach (Slot slot in SlotList)
        {
            if (slot.ItemStack.ID == itemStack.ID)
            {
                if (slot.ItemStack.Quantity + itemStack.Quantity > slot.ItemStack.MaxQuantity)
                {
                    int excessQuantity = slot.ItemStack.Quantity + itemStack.Quantity - slot.ItemStack.MaxQuantity;
                    slot.ItemStack.SetQuantity(slot.ItemStack.MaxQuantity);
                    itemStack.SetQuantity(excessQuantity);
                }
                else
                {
                    slot.ItemStack.TryAdd(itemStack.Quantity);
                    return null;
                }
            }
        }
        
        foreach (Slot slot in SlotList)
        {
            if (slot.IsEmpty)
            {
                slot.AddItem(itemStack);
                return null;
            }
        }

        return itemStack;
    }
}
