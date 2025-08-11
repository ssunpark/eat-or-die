using UnityEngine;
using Fusion;

public class SharedStorage : NetworkBehaviour
{
    private const int STORAGE_SIZE = 60;

    [Networked, Capacity(STORAGE_SIZE)] public NetworkArray<NetworkedItem> Items { get; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TryTakeItem(int slotIndex, RpcInfo info = default)
    {
        if (slotIndex < 0 || slotIndex >= STORAGE_SIZE) return;

        NetworkedItem itemInSlot = Items.Get(slotIndex);

        if (itemInSlot.ID == 0) return;

        RPC_PutItemToLocal(info.Source, itemInSlot);

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
            int remainingQuantity = itemInSlot.Quantity - 1;
            
            Items.Set(slotIndex, new NetworkedItem
            {
                ID = itemInSlot.ID,
                Quantity = remainingQuantity,
                Durability = itemInSlot.Durability,
                MaxQuantity = itemInSlot.MaxQuantity
            });

            RPC_PutItemToLocal(info.Source, new NetworkedItem
            {
                ID = itemInSlot.ID,
                Quantity = 1,
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
            RPC_PutItemToLocal(info.Source, itemInSlot);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PutItemToLocal([RpcTarget] PlayerRef player, NetworkedItem item)
    {
        ItemProfile itemProfile = ItemManager.Instance.GetItem(item.ID);
        ItemInstance itemInstance = new ItemInstance(itemProfile, item.Quantity, item.Durability);

        SharedStorageManager.Instance.GetItemFromStorage(itemInstance);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TryPutItem(int slotIndex, NetworkedItem item, PlayerRef player)
    {
        if (slotIndex < 0 || slotIndex >= STORAGE_SIZE) return;
     
        // 원래 있던 아이템
        NetworkedItem itemInSlot = Items.Get(slotIndex);

        if (itemInSlot.ID == 0)
        {
            Items.Set(slotIndex, item);
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
                RPC_PutItemToLocal(player, remainItem);
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
            }
        }
        else
        {
            RPC_PutItemToLocal(player, itemInSlot);
            Items.Set(slotIndex, item);
        }
    }
}
