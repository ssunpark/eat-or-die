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
            if (HandEntity.Instance.GetItem()?.ItemProfile?.ItemDefinition == null)
            {
                return;
            }

            if (HandEntity.Instance.GetItem().ItemProfile.ItemDefinition.Type != EItemType.Extra)
            {
                UI_Notification.Notify("플레이어의 시체만 부활시킬 수 있습니다.");
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
                if (HandEntity.Instance.GetItem().ItemProfile.ItemDefinition.Type != EItemType.Extra)
                {
                    UI_Notification.Notify("플레이어의 시체만 부활시킬 수 있습니다.");
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

    public void TryRevive()
    {
        if (ReviveShopInventory.SlotList[0].IsEmpty)
        {
            UI_Notification.Notify("부활시킬 플레이어가 없습니다.");
            return;
        }
        var reviveItem = ReviveShopInventory.SlotList[0].ItemInstance;
        string extraInfo = reviveItem.ExtraInfo;

        if (string.IsNullOrEmpty(extraInfo))
        {
            Debug.LogError("부활 아이템에 플레이어 정보가 없습니다.");
            return;
        }
        Debug.Log(extraInfo);
        var foundedPlayer = PlayerInfoManager.Instance.GetPlayerFromCharacterId(extraInfo);

        if(foundedPlayer == null)
        {
            UI_Notification.Notify("해당 플레이어를 찾을 수 없습니다.");
            ReviveShopInventory.PopItemInSlot(0);
            OnReviveSlotUpdated[0]?.Invoke();
            return;
        }

        if (!foundedPlayer.IsDead)
        {
            UI_Notification.Notify("이미 부활한 플레이어입니다.");
            ReviveShopInventory.PopItemInSlot(0);
            OnReviveSlotUpdated[0]?.Invoke();
            return;
        }

        Debug.Log($"[ReviveShopManager] Reviving player: {foundedPlayer.NetworkObject.InputAuthority}");
        foundedPlayer.Rpc_RequestRevive();
        ReviveShopInventory.PopItemInSlot(0);

        OnReviveSlotUpdated[0]?.Invoke();

    }
}