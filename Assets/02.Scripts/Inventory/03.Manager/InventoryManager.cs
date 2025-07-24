using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    private Inventory _inventory;
    public Inventory Inventory => _inventory;
    public int InventorySize;
    
    public List<Action> OnSlotUpdated;
    public Action OnInventoryUpdated;

    private void Awake()
    {
        _inventory = new Inventory(InventorySize);
        OnSlotUpdated = new List<Action>(new Action[InventorySize]);
    }

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            Item itemInSlot = _inventory.PopItemInSlot(slotIndex);
            if (itemInSlot == null) return;
            
            HandEntity.Instance.PickUpItem(itemInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(_inventory.PutItemInSlot(slotIndex, HandEntity.Instance.Item));
        }
        OnSlotUpdated[slotIndex]?.Invoke();
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
            if (HandEntity.Instance.Item.ID == _inventory.SlotList[slotIndex].Item.ID)
            {
                Item itemInSlot = _inventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInSlot))
                {
                    _inventory.SlotList[slotIndex].Item.TryAdd(itemInSlot.Quantity);
                }
            }
            else
            {
                Item temp = _inventory.PopItemInSlot(slotIndex);
                _inventory.PutItemInSlot(slotIndex, HandEntity.Instance.Item);
                HandEntity.Instance.PickUpItem(temp);
            }
        }

        OnSlotUpdated[slotIndex]?.Invoke();
        OnInventoryUpdated?.Invoke();
    }

    public void PickItemFromGround(Item item)
    {
        Item remain = _inventory.PickItemFromGround(item);
        
        OnInventoryUpdated?.Invoke();
     
        if (remain == null) return;
        
        ItemManager.Instance.RPC_CreateItemObject(remain.ID, remain.Quantity, Vector3.zero, Quaternion.identity);
    }
}
