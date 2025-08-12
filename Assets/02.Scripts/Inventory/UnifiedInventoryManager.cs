using System;
using UnityEngine;

public class UnifiedInventoryManager : BehaviourSingleton<UnifiedInventoryManager>
{
    public event Action OnPossessionUpdated; // 소지품 변경 이벤트
    public event Action<ItemInstance> OnItemAcquired; // 아이템 획득 이벤트

    private void Start()
    {
        SharedStorageManager.Instance.OnStorageUpdated += OnPossessionUpdated; // 창고 내용물이 바뀔 때
        HandEntity.Instance.OnItemDropped += OnPossessionUpdated; // 아이템을 필드에 드랍할 때
    }

    public void DropAllItems()
    {
        InventoryManager.Instance.DropAllItems();
        QuickSlotManager.Instance.DropAllItems();
        
        OnPossessionUpdated?.Invoke();
    }

    public void AddItem(ItemInstance itemInstance)
    {
        ItemInstance remain = QuickSlotManager.Instance.AddItemToQuickSlot(itemInstance);

        if (remain != null)
        {
            remain = InventoryManager.Instance.AddItemToInventory(remain);
        }

        if (remain != null)
        {
            remain = QuickSlotManager.Instance.AddItemToEmptySlot(remain);
        }

        if (remain != null)
        {
            remain = InventoryManager.Instance.AddItemToEmptySlot(remain);
        }
        
        if (remain != null)
        {
            ItemManager.Instance.RPC_CreateItemObject(
                remain.ID,
                remain.Quantity,
                remain.Durability,
                Room.Instance.LocalPlayer.transform.position,
                Quaternion.identity);
        }
        
        OnItemAcquired?.Invoke(itemInstance);
        OnPossessionUpdated?.Invoke();
    }

    public bool HaveItem(int itemID)
    {
        return InventoryManager.Instance.HaveItem(itemID)
            || QuickSlotManager.Instance.HaveItem(itemID)
            || SharedStorageManager.Instance.HaveItem(itemID);
    }
    
    public int GetItemCount(int itemID)
    {
        return InventoryManager.Instance.GetItemCount(itemID)
             + QuickSlotManager.Instance.GetItemCount(itemID)
             + SharedStorageManager.Instance.GetItemCount(itemID);
    }
}