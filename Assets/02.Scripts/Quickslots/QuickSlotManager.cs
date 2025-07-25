using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : BehaviourSingleton<QuickSlotManager>
{
	public Inventory QuickSlots;
	public int QuickSlotSize;

	private int _selectedSlotIndex;
	
	public Action<int> OnQuickSlotUpdated;
	
	private void Awake()
	{
		QuickSlots = new Inventory(QuickSlotSize);
	}
	
	public void OnSelectSlot(int slotIndex)
	{
		if (!PopupManager.Instance.IsOpen(EPopupType.Inventory))
		{
			if (_selectedSlotIndex == slotIndex) return;

			if (!QuickSlots.SlotList[_selectedSlotIndex].IsEmpty)
			{
				AItemInfo itemInPlayerHand = ItemManager.Instance.GetItem(QuickSlots.SlotList[_selectedSlotIndex].Item.ID);
				if (itemInPlayerHand is IEquipable equipped)
				{
					equipped.Unequip(Room.Instance.LocalPlayer);
				}
			}
			
			_selectedSlotIndex = slotIndex;
			AItemInfo itemInfoInSlot = ItemManager.Instance.GetItem(QuickSlots.SlotList[slotIndex].Item.ID);
			if (itemInfoInSlot is IEquipable equipItem)
			{
                equipItem.Equip(Room.Instance.LocalPlayer);
			}
			else
			{
				Debug.Log("Item is not equippable: " + itemInfoInSlot.ItemData.Name);
			}
			return;
		}
		
		if (HandEntity.Instance.IsHandEmpty)
		{
			Item itemInSlot = QuickSlots.PopItemInSlot(slotIndex);
			if (itemInSlot == null) return;
            
			HandEntity.Instance.PickUpItem(itemInSlot);
		}
		else
		{
			HandEntity.Instance.PickUpItem(QuickSlots.PutItemInSlot(slotIndex, HandEntity.Instance.Item));
		}
		OnQuickSlotUpdated?.Invoke(slotIndex);
	}

	public void OnClickMouseRight(int slotIndex)
	{
		if (QuickSlots.SlotList[slotIndex].IsEmpty) return;
        
		if (HandEntity.Instance.IsHandEmpty)
		{
			HandEntity.Instance.PickUpItem(QuickSlots.PopSingleItemInSlot(slotIndex));
		}
		else
		{
			if (HandEntity.Instance.Item.ID == QuickSlots.SlotList[slotIndex].Item.ID)
			{
				Item itemInSlot = QuickSlots.PopSingleItemInSlot(slotIndex);
				if (!HandEntity.Instance.TryAddItem(itemInSlot))
				{
					QuickSlots.SlotList[slotIndex].Item.TryAdd(itemInSlot.Quantity);
				}
			}
			else
			{
				Item temp = QuickSlots.PopItemInSlot(slotIndex);
				QuickSlots.PutItemInSlot(slotIndex, HandEntity.Instance.Item);
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
