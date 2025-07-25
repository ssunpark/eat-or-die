using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class QuickSlotManager : BehaviourSingleton<QuickSlotManager>
{
	private Inventory _quickSlots;
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
	
	public void UpdateSelectedSlot()
	{
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
		
		if (item == null)
		{
			Debug.Log("Selected slot is empty.");
			//Room.Instance.LocalPlayer.UnequipItem();
		}
		else
		{
			Debug.Log("Sending item to player: " + item.ID);
			// Room.Instance.LocalPlayer.EquipItem(item);
		}
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
    
    // 플레이어가 접근해서 호출
    // 손에 든 아이템 사용 함수 추가
    // 사용 시 내구도 및 갯수 감소
    // 아이템 사용
    // UI 갱신
}
