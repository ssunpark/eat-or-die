using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    private Inventory _inventory;
    public int InventorySize;
    
    public event Action<int> OnSlotUpdated;
    public event Action OnInventoryUpdated;

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

    public ItemInstance AddItemToInventory(ItemInstance itemInstance)
    {
        ItemInstance remain = _inventory.AddItemToInventory(itemInstance);
        
        OnInventoryUpdated?.Invoke();
        
        return remain;
    }

    public ItemInstance AddItemToEmptySlot(ItemInstance itemInstance)
    {
        ItemInstance remain = _inventory.AddItemToEmptySlot(itemInstance);
        
        OnInventoryUpdated?.Invoke();

        return remain;
    }

    public bool HaveItem(int itemID)
    {
        return _inventory.HaveItem(itemID);
    }

    public int GetItemCount(int itemID)
    {
        return _inventory.GetItemCount(itemID);
    }
    
    public bool TryConsumeItem(int itemID, int amount)
    {
        bool result = _inventory.TryConsumeItem(itemID, amount);
        
        if (result)
        {
            OnInventoryUpdated?.Invoke();
        }
        return result;
    }

    public ItemInstance GetItemInSlot(int slotIndex)
    {
        return _inventory.GetItemInSlot(slotIndex);
    }
    
    public List<Slot> GetAllSlots()
    {
        return _inventory.GetAllSlots();
    }

    public void DropAllItems(Vector3 position)
    {
        List<Slot> slots = GetAllSlots();

        foreach (Slot slot in slots)
        {
            if (!slot.IsEmpty)
            {
                ItemInstance item = slot.GetItem();
                ItemManager.Instance.RPC_CreateItemObject(item.ID, item.Quantity, item.Durability, position, Quaternion.identity);
                slot.RemoveItem();
            }
        }
        OnInventoryUpdated?.Invoke();
    }
}
