using System;
using UnityEngine;

public class QuickSlotManager : BehaviourSingleton<QuickSlotManager>
{
	private Inventory _quickSlots;
	public Inventory Quickslots => _quickSlots;
    public int QuickSlotSize;

	private int _selectedSlotIndex;
	
	public Action<int> OnQuickSlotUpdated;
	
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
        
        OnQuickSlotUpdated?.Invoke(_selectedSlotIndex);
    }

	public Item GetItemInSlot(int slotIndex)
	{
		Item item = _quickSlots.GetItemInSlot(slotIndex);

		return item;
	}
	
	public void SendItemToPlayer()
	{
		Item item = GetItemInSlot(_selectedSlotIndex);

        Room.Instance.LocalPlayer.GetComponent<PlayerItemHolder>().SetHoldItem(item);
		
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
			Item itemInHand = hand.GetItem();
			hand.PickUpItem(_quickSlots.PutItemInSlot(_selectedSlotIndex, itemInHand));
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
			if (HandEntity.Instance.Item.ID == _quickSlots.SlotList[slotIndex].Item.ID)
			{
				Item itemInSlot = _quickSlots.PopSingleItemInSlot(slotIndex);
				if (!HandEntity.Instance.TryAddItem(itemInSlot))
				{
					_quickSlots.SlotList[slotIndex].Item.TryAdd(itemInSlot.Quantity);
				}
			}
			else
			{
				Item temp = _quickSlots.PopItemInSlot(slotIndex);
				_quickSlots.PutItemInSlot(slotIndex, HandEntity.Instance.Item);
				HandEntity.Instance.PickUpItem(temp);
			}
		}

		OnQuickSlotUpdated?.Invoke(slotIndex);
	}
}
