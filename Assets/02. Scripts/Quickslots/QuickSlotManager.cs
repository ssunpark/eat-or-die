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
		// 팝업 비활성화시 아이템을 선택하게 하는 로직 추가
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
