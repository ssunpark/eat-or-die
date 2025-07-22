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
			AItem itemInSlot = ItemManager.Instance.GetItem(QuickSlots.SlotList[slotIndex].ItemStack.ID);
			if (itemInSlot is IEquipable equipItem)
			{
				Debug.Log("Equipping item: " + equipItem);
                equipItem.Equip(Room.Instance.LocalPlayer);
			}
			else
			{
				Debug.Log("Item is not equippable: " + itemInSlot.ItemData.Name);
			}
			return;
		}
		
		if (HandEntity.Instance.IsHandEmpty)
		{
			ItemStack itemInSlot = QuickSlots.PopItemInSlot(slotIndex);
			if (itemInSlot == null) return;
            
			HandEntity.Instance.PickUpItem(itemInSlot);
		}
		else
		{
			HandEntity.Instance.PickUpItem(QuickSlots.PutItemInSlot(slotIndex, HandEntity.Instance.ItemStack));
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
			if (HandEntity.Instance.ItemStack.ID == QuickSlots.SlotList[slotIndex].ItemStack.ID)
			{
				ItemStack itemInSlot = QuickSlots.PopSingleItemInSlot(slotIndex);
				if (!HandEntity.Instance.TryAddItem(itemInSlot))
				{
					QuickSlots.SlotList[slotIndex].ItemStack.TryAdd(itemInSlot.Quantity);
				}
			}
			else
			{
				ItemStack temp = QuickSlots.PopItemInSlot(slotIndex);
				QuickSlots.PutItemInSlot(slotIndex, HandEntity.Instance.ItemStack);
				HandEntity.Instance.PickUpItem(temp);
			}
		}

		OnQuickSlotUpdated[slotIndex]?.Invoke();
	}
}
