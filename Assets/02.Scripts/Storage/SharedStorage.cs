using System;
using UnityEngine;
using Fusion;

public class SharedStorage : NetworkBehaviour
{
    private const int STORAGE_SIZE = 30;
    
    public event Action OnStorageUpdated;
    
    [Networked, Capacity(STORAGE_SIZE)] public NetworkArray<NetworkedItem> Items { get; }

    private ChangeDetector _changeDetector;
    
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public bool HaveItem(int ItemID)
    {
        foreach (NetworkedItem networkedItem in Items)
        {
            if (networkedItem.ID == ItemID && networkedItem.Quantity > 0)
            {
                return true;
            }
        }

        return false;
    }
    
    public int GetItemCount(int itemID)
    {
        int count = 0;
        
        foreach (NetworkedItem networkedItem in Items)
        {
            if (networkedItem.ID == itemID)
            {
                count += networkedItem.Quantity;
            }
        }
        return count;
    }
    
    public override void Render()
    {
        foreach (string change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Items):
                    OnStorageUpdated?.Invoke();
                    break;
            }
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TryTakeItem(int slotIndex, RpcInfo info = default)
    {
        if (slotIndex < 0 || slotIndex >= STORAGE_SIZE) return;

        NetworkedItem itemInSlot = Items.Get(slotIndex);

        if (itemInSlot.ID == 0) return;

        RPC_ItemToLocal(info.Source, itemInSlot);

        Items.Set(slotIndex, new NetworkedItem { ID = 0 });
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TryTakeOneItem(int slotIndex, NetworkedItem itemInHand, RpcInfo info = default)
    {
        if (slotIndex < 0 || slotIndex >= STORAGE_SIZE) return;

        NetworkedItem itemInSlot = Items.Get(slotIndex);
        
        if (itemInSlot.ID == 0) return;

        if (itemInHand.ID == 0 || itemInHand.ID == itemInSlot.ID)
        {
            if (itemInHand.ID != 0 && itemInHand.Quantity >= itemInHand.MaxQuantity) return;
            
            int remainingQuantity = itemInSlot.Quantity - 1;
            
            Items.Set(slotIndex, new NetworkedItem
            {
                ID = itemInSlot.ID,
                Quantity = remainingQuantity,
                Durability = itemInSlot.Durability,
                MaxQuantity = itemInSlot.MaxQuantity
            });

            RPC_ItemToLocal(info.Source, new NetworkedItem
            {
                ID = itemInSlot.ID,
                Quantity = itemInHand.Quantity + 1,
                Durability = itemInSlot.Durability,
                MaxQuantity = itemInSlot.MaxQuantity
            });
            
            if (remainingQuantity <= 0)
            {
                Items.Set(slotIndex, new NetworkedItem { ID = 0 });
            }
        }
        else
        {
            Items.Set(slotIndex, itemInHand);
            RPC_ItemToLocal(info.Source, itemInSlot);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ItemToLocal([RpcTarget] PlayerRef player, NetworkedItem item)
    {
        if (item.ID == 0)
        {
            SharedStorageManager.Instance.GetItemFromStorage(null);
            return;
        }
        
        ItemProfile itemProfile = ItemManager.Instance.GetItem(item.ID);
        ItemInstance itemInstance = new ItemInstance(itemProfile, item.Quantity, item.Durability);

        SharedStorageManager.Instance.GetItemFromStorage(itemInstance);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TryPutItem(int slotIndex, NetworkedItem item, RpcInfo info = default)
    {
        if (slotIndex < 0 || slotIndex >= STORAGE_SIZE) return;
     
        // 원래 있던 아이템
        NetworkedItem itemInSlot = Items.Get(slotIndex);

        if (itemInSlot.ID == 0)
        {
            Items.Set(slotIndex, item);
            RPC_ItemToLocal(info.Source, new NetworkedItem { ID = 0 });
        }
        else if (itemInSlot.ID == item.ID) // 같은 아이템인 경우
        {
            if (item.Quantity + itemInSlot.Quantity > item.MaxQuantity)
            {
                int remainQuantity = item.Quantity + itemInSlot.Quantity - item.MaxQuantity;
                
                Items.Set(slotIndex, new NetworkedItem
                {
                    ID = item.ID,
                    Quantity = item.MaxQuantity,
                    Durability = itemInSlot.Durability,
                    MaxQuantity = item.MaxQuantity
                });

                NetworkedItem remainItem = new NetworkedItem
                {
                    ID = item.ID,
                    Quantity = remainQuantity,
                    Durability = item.Durability,
                    MaxQuantity = item.MaxQuantity
                };
                RPC_ItemToLocal(info.Source, remainItem);
            }
            else
            {
                Items.Set(slotIndex, new NetworkedItem
                {
                    ID = itemInSlot.ID,
                    Quantity = itemInSlot.Quantity + item.Quantity,
                    Durability = itemInSlot.Durability,
                    MaxQuantity = itemInSlot.MaxQuantity
                });
                RPC_ItemToLocal(info.Source, new NetworkedItem { ID = 0 });
            }
        }
        else
        {
            Items.Set(slotIndex, item);
            RPC_ItemToLocal(info.Source, itemInSlot);
        }
    }

    public NetworkedItem GetItemInSlot(int slotIndex)
    {
        return Items.Get(slotIndex);
    }
}
