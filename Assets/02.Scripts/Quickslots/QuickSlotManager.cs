using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : BehaviourSingleton<QuickSlotManager>
{
	private Inventory _quickSlots;
    public int QuickSlotSize;

	private int _selectedSlotIndex;
	
	public event Action OnEntireQuickSlotUpdated;
	public event Action<int> OnQuickSlotUpdated;
	public event Action OnUseItem;
	
	private void Awake()
	{
		_quickSlots = new Inventory(QuickSlotSize);
	}

	private void SetSelectedSlot(int slotIndex)
	{
		_selectedSlotIndex = slotIndex;
		OnQuickSlotUpdated?.Invoke(_selectedSlotIndex);
	}

    public void UseItem(GameObject target, Action removeCallback)
    {
        var currentItem = GetItemInSlot(_selectedSlotIndex);
        currentItem.Use(target); 
        
        // 아이템 전부 소진
        if (currentItem.IsDepleted)
        {
            _quickSlots.SlotList[_selectedSlotIndex].RemoveItem();
            removeCallback?.Invoke();
        }
        
        OnUseItem?.Invoke();
        OnQuickSlotUpdated?.Invoke(_selectedSlotIndex);
    }
    
    public int RequestConsumeItem(int itemID, int amount)
    {
	    int consumed = GetItemCount(itemID);

	    if (amount < consumed)
	    {
		    consumed = amount;
	    }
        
	    TryConsumeItem(itemID, consumed);
        
	    return consumed;
    }
    
    public bool TryConsumeItem(int itemID, int amount)
    {
	    bool result = _quickSlots.TryConsumeItem(itemID, amount);
        
	    if (result)
	    {
		    OnEntireQuickSlotUpdated?.Invoke();
	    }
	    return result;
	}

	public ItemInstance GetItemInSlot(int slotIndex)
	{
		ItemInstance itemInstance = _quickSlots.GetItemInSlot(slotIndex);

		return itemInstance;
	}
	
	public void SendItemToPlayer()
	{
		ItemInstance itemInstance = GetItemInSlot(_selectedSlotIndex);

        Room.Instance.LocalPlayer.GetComponent<PlayerItemHolder>().SetHoldItem(itemInstance);
		
	}

	private void HandSwap()
	{
		HandEntity hand = HandEntity.Instance;
		
		if (hand.IsHandEmpty)
		{
			hand.PickUpItem(_quickSlots.PopItemInSlot(_selectedSlotIndex));
		}
		else
		{
			ItemInstance itemInstanceInHand = hand.GetItem();
			hand.PickUpItem(_quickSlots.PutItemInSlot(_selectedSlotIndex, itemInstanceInHand));
		}
		OnQuickSlotUpdated?.Invoke(_selectedSlotIndex);
	}

	public void OnClickMouseLeft(int slotIndex)
	{
		SetSelectedSlot(slotIndex);

		if (PopupManager.Instance.IsOpen(EPopupType.Inventory))
		{
			HandSwap();
		}
		
		SendItemToPlayer();
	}
	
	public ItemInstance AddItemToQuickSlot(ItemInstance itemInstance)
	{
		ItemInstance remain = _quickSlots.AddItemToInventory(itemInstance);
        
		OnEntireQuickSlotUpdated?.Invoke();

		return remain;
	}
	
	public ItemInstance AddItemToEmptySlot(ItemInstance itemInstance)
	{
		ItemInstance remain = _quickSlots.AddItemToEmptySlot(itemInstance);
        
		OnEntireQuickSlotUpdated?.Invoke();

		return remain;
	}

	public void OnClickMouseRight(int slotIndex)
	{
		if (!PopupManager.Instance.IsOpen(EPopupType.Inventory)) return;
		
		if (_quickSlots.SlotList[slotIndex].IsEmpty) return;
        
		if (HandEntity.Instance.IsHandEmpty)
		{
			HandEntity.Instance.PickUpItem(_quickSlots.PopSingleItemInSlot(slotIndex));
		}
		else
		{
			if (HandEntity.Instance.ItemInstance.ID == _quickSlots.SlotList[slotIndex].ItemInstance.ID)
			{
				ItemInstance itemInstanceInSlot = _quickSlots.PopSingleItemInSlot(slotIndex);
				if (!HandEntity.Instance.TryAddItem(itemInstanceInSlot))
				{
					_quickSlots.SlotList[slotIndex].ItemInstance.TryAdd(itemInstanceInSlot.Quantity);
				}
			}
			else
			{
				ItemInstance temp = _quickSlots.PopItemInSlot(slotIndex);
				_quickSlots.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance);
				HandEntity.Instance.PickUpItem(temp);
			}
		}

		OnQuickSlotUpdated?.Invoke(slotIndex);
	}

	public List<Slot> GetAllSlots()
	{
		return _quickSlots.GetAllSlots();
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
		OnEntireQuickSlotUpdated?.Invoke();
	}
	
	public bool HaveItem(int itemID)
	{
		return _quickSlots.HaveItem(itemID);
	}
	
	public int GetItemCount(int itemID)
	{
		return _quickSlots.GetItemCount(itemID);
	}
}
