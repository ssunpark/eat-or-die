using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    private Inventory _inventory;
    public Inventory Inventory => _inventory;
    public int InventorySize;
    
    public event Action<int> OnSlotUpdated;
    public event Action OnInventoryUpdated;
    public static event Action<ItemInstance> OnItemAcquired;

    private void Awake()
    {
        _inventory = new Inventory(InventorySize);
    }

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            ItemInstance itemInstanceInSlot = _inventory.PopItemInSlot(slotIndex);
            if (itemInstanceInSlot == null) return;
            
            HandEntity.Instance.PickUpItem(itemInstanceInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(_inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance));
        }
        OnSlotUpdated?.Invoke(slotIndex);
        OnInventoryUpdated?.Invoke();
    }
    
    public void OnClickMouseRight(int slotIndex)
    {
        if (_inventory.SlotList[slotIndex].IsEmpty) return;
        
        if (HandEntity.Instance.IsHandEmpty)
        {
            HandEntity.Instance.PickUpItem(_inventory.PopSingleItemInSlot(slotIndex));
        }
        else
        {
            if (HandEntity.Instance.ItemInstance.ID == _inventory.SlotList[slotIndex].ItemInstance.ID)
            {
                ItemInstance itemInstanceInSlot = _inventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInstanceInSlot))
                {
                    _inventory.SlotList[slotIndex].ItemInstance.TryAdd(itemInstanceInSlot.Quantity);
                }
            }
            else
            {
                ItemInstance temp = _inventory.PopItemInSlot(slotIndex);
                _inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance);
                HandEntity.Instance.PickUpItem(temp);
            }
        }

        OnSlotUpdated?.Invoke(slotIndex);
        OnInventoryUpdated?.Invoke();
    }

    public void PickItemFromGround(ItemInstance itemInstance)
    {
        ItemInstance remain = _inventory.PickItemFromGround(itemInstance);
        
        OnInventoryUpdated?.Invoke();
        OnItemAcquired?.Invoke(itemInstance);
        if (remain == null) return;
        
        ItemManager.Instance.RPC_CreateItemObject(remain.ID, remain.Quantity, remain.Durability, Vector3.zero, Quaternion.identity);
    }
}
