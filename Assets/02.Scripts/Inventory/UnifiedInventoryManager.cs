using System;
using UnityEngine;

public class UnifiedInventoryManager : BehaviourSingleton<UnifiedInventoryManager>
{
    public event Action OnPossessionUpdated; // 소지품 변경 이벤트
    public event Action<ItemInstance> OnItemAcquired; // 아이템 획득 이벤트

    private void Start()
    {
        QuickSlotManager.Instance.OnUseItem += InvokeOnPossessionUpdated; // 퀵슬롯 아이템 사용 시
        SharedStorageManager.Instance.OnStorageUpdated += InvokeOnPossessionUpdated; // 창고 내용물이 바뀔 때
        HandEntity.Instance.OnItemDropped += InvokeOnPossessionUpdated; // 아이템을 필드에 드랍할 때
    }

    public void InvokeOnPossessionUpdated()
    {
        OnPossessionUpdated?.Invoke();
    }

    public void DropAllItems(Vector3 position)
    {
        InventoryManager.Instance.DropAllItems(position);
        QuickSlotManager.Instance.DropAllItems(position);
        
        OnPossessionUpdated?.Invoke();
    }

    public void AddItem(ItemInstance itemInstance)
    {
        OnItemAcquired?.Invoke(itemInstance);
        
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
            ItemProxySpawner.Instance.RPC_CreateItemObject(
                remain.ID,
                remain.Quantity,
                remain.Durability,
                Room.Instance.LocalPlayer.transform.position,
                Quaternion.identity);
        }
        
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
        return GetLocalItemCount(itemID) + GetNetworkedItemCount(itemID);
    }
    
    public int GetLocalItemCount(int itemID)
    {
        return InventoryManager.Instance.GetItemCount(itemID)
             + QuickSlotManager.Instance.GetItemCount(itemID);
    }
    
    public int GetNetworkedItemCount(int itemID)
    {
        return SharedStorageManager.Instance.GetItemCount(itemID);
    }

    public bool TryConsumeLocalItem(int itemID, int amount)
    {
        if (GetLocalItemCount(itemID) < amount)
        {
            return false;
        }
        
        int consumed = QuickSlotManager.Instance.RequestConsumeItem(itemID, amount);

        if (consumed < amount)
        {
            consumed += InventoryManager.Instance.RequestConsumeItem(itemID, amount - consumed);
        }
        
        OnPossessionUpdated?.Invoke();

        return consumed == amount;
    }
}