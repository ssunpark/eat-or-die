using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;

public class ReviveShopManager : NetworkBehaviourSingleton<ReviveShopManager>
{
    public Inventory ReviveShopInventory = new Inventory(1);
    public List<Action> OnReviveSlotUpdated = new List<Action>(new Action[1]);

    public bool IsSpawned => Object != null && Object.IsValid;

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            var itemInSlot = ReviveShopInventory.PopItemInSlot(slotIndex);
            if (itemInSlot == null) return;
            HandEntity.Instance.PickUpItem(itemInSlot);
        }
        else
        {
            if (HandEntity.Instance.GetItem().ItemProfile.ItemDefinition.ID != 1800001)
            {
                UI_Notification.Notify("시체 아이템만 넣을 수 있습니다.");
                return;
            }
            
            HandEntity.Instance.PickUpItem(ReviveShopInventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance));
        }
        OnReviveSlotUpdated[slotIndex]?.Invoke();
    }

    public void OnClickMouseRight(int slotIndex)
    {
        if (ReviveShopInventory.SlotList[slotIndex].IsEmpty) return;

        if (HandEntity.Instance.IsHandEmpty)
        {
            HandEntity.Instance.PickUpItem(ReviveShopInventory.PopSingleItemInSlot(slotIndex));
        }
        else
        {
            if (HandEntity.Instance.ItemInstance.ID == ReviveShopInventory.SlotList[slotIndex].ItemInstance.ID)
            {
                var itemInSlot = ReviveShopInventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInSlot))
                {
                    ReviveShopInventory.SlotList[slotIndex].ItemInstance.TryAdd(itemInSlot.Quantity);
                }
            }
            else
            {
                if (HandEntity.Instance.GetItem().ItemProfile.ItemDefinition.ID != 1800001)
                {
                    UI_Notification.Notify("시체 아이템만 넣을 수 있습니다.");
                    return;
                }
                
                ItemInstance reviveItem = ReviveShopInventory.PopItemInSlot(slotIndex);
                ReviveShopInventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance);
                HandEntity.Instance.PickUpItem(reviveItem);
            }
        }
        OnReviveSlotUpdated[slotIndex]?.Invoke();
    }
    
    public bool HasEmptySlot()
    {
        return ReviveShopInventory.SlotList.Exists(slot => slot.IsEmpty);
    }
}