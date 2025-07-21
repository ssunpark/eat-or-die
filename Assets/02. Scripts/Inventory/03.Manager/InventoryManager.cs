using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    public Inventory Inventory;
    public int InventorySize;
    
    public List<Action> OnSlotUpdated;
    public Action OnInventoryUpdated;

    private void Awake()
    {
        Inventory = new Inventory(InventorySize);
        OnSlotUpdated = new List<Action>(new Action[InventorySize]);
    }

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            ItemStack itemInSlot = Inventory.PopItemInSlot(slotIndex);
            if (itemInSlot == null) return;
            
            HandEntity.Instance.PickUpItem(itemInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(Inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemStack));
        }
        OnSlotUpdated[slotIndex]?.Invoke();
        OnInventoryUpdated?.Invoke();
    }
    
    public void OnClickMouseRight(int slotIndex)
    {
        if (Inventory.SlotList[slotIndex].IsEmpty) return;
        
        if (HandEntity.Instance.IsHandEmpty)
        {
            HandEntity.Instance.PickUpItem(Inventory.PopSingleItemInSlot(slotIndex));
        }
        else
        {
            if (HandEntity.Instance.ItemStack.ID == Inventory.SlotList[slotIndex].ItemStack.ID)
            {
                ItemStack itemInSlot = Inventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInSlot))
                {
                    Inventory.SlotList[slotIndex].ItemStack.TryAdd(itemInSlot.Quantity);
                }
            }
            else
            {
                ItemStack temp = Inventory.PopItemInSlot(slotIndex);
                Inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemStack);
                HandEntity.Instance.PickUpItem(temp);
            }
        }

        OnSlotUpdated[slotIndex]?.Invoke();
        OnInventoryUpdated?.Invoke();
    }

    public void PickItemFromGround(ItemStack itemStack)
    {
        ItemStack remain = Inventory.PickItemFromGround(itemStack);
        
        OnInventoryUpdated?.Invoke();
     
        if (remain == null) return;
        
        ItemManager.Instance.RPC_CreateItemObject(remain.ID, remain.Quantity, Vector3.zero, Quaternion.identity);
    }
}
