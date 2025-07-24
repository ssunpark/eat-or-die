using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : BehaviourSingleton<QuickSlotManager>
{
	public Inventory QuickSlots;
	public int QuickSlotSize;
	public List<Action> OnQuickSlotUpdated = new List<Action>();
	
	private void Awake()
	{
		QuickSlots = new Inventory(QuickSlotSize);
		OnQuickSlotUpdated = new List<Action>(new Action[QuickSlotSize]);
	}
	
	public void OnClickMouseLeft(int slotIndex)
	{
		if (!PopupManager.Instance.IsOpen(EPopupType.Inventory))
		{
			if (QuickSlots.SlotList[slotIndex].IsEmpty)
			{
				Room.Instance.LocalPlayer.GetComponent<FarmingInteractionTest>().OnUnequipped();
				return;
			}
			
			// 슬롯의 아이템 타입에 따라 적절한 메서드를 호출해야 합니다. 근데 지금은 연결할 로직이 없음
			AItemInfo itemInfoInSlot = ItemManager.Instance.GetItem(QuickSlots.SlotList[slotIndex].Item.ID);
			if (itemInfoInSlot is IEquipable equipItem)
			{
				Debug.Log("Equipping item: " + equipItem);
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
		OnQuickSlotUpdated[slotIndex]?.Invoke();
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

		OnQuickSlotUpdated[slotIndex]?.Invoke();
	}
    
    // 플레이어가 접근해서 호출
    // 손에 든 아이템 사용 함수 추가
    // 사용 시 내구도 및 갯수 감소
    // 아이템 사용
    // UI 갱신
}
