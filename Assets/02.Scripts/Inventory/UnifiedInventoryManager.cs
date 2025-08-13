using System;
using UnityEngine;

public class UnifiedInventoryManager : BehaviourSingleton<UnifiedInventoryManager>
{
    public event Action OnPossessionUpdated; // 소지품 변경 이벤트
    public event Action<ItemInstance> OnItemAcquired; // 아이템 획득 이벤트

    private void Start()
    {
        QuickSlotManager.Instance.OnUseItem += OnPossessionUpdated;
        SharedStorageManager.Instance.OnStorageUpdated += OnPossessionUpdated; // 창고 내용물이 바뀔 때
        HandEntity.Instance.OnItemDropped += OnPossessionUpdated; // 아이템을 필드에 드랍할 때
    }

    public void DropAllItems(Vector3 position)
    {
        InventoryManager.Instance.DropAllItems(position);
        QuickSlotManager.Instance.DropAllItems(position);
        
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

    // public bool TryConsumeItem(int itemID, int amount)
    // {
    //     // 로컬 아이템 갯수가 더 많으면 그냥 쓰고 종료
    //     // 로컬 아이템이 부족하면 네트워크 창고에서 아이템 회수 시도
    //     // 회수 후 아이템 부족하면 누가 먼저 가져간거
    //     
    //     OnPossessionUpdated?.Invoke();
    // }
}