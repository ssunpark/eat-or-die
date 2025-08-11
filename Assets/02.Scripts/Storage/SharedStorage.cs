using UnityEngine;
using Fusion;

public class SharedStorage : NetworkBehaviour
{
    private const int STORAGE_SIZE = 60;
 
    [Networked, Capacity(STORAGE_SIZE)]
    public NetworkArray<NetworkedItem> Items { get; }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TryTakeItem(int slotIndex, PlayerRef player)
    {
        if (slotIndex < 0 || slotIndex >= STORAGE_SIZE) return;
    
        NetworkedItem itemInSlot = Items.Get(slotIndex);
        
        if (itemInSlot.ID == 0) return;
        
        RPC_PutItemInHand(player, itemInSlot);

        Items.Set(slotIndex, new NetworkedItem { ID = 0 });
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PutItemInHand([RpcTarget] PlayerRef player, NetworkedItem item)
    {
        ItemProfile itemProfile = ItemManager.Instance.GetItem(item.ID);
        ItemInstance itemInstance = new ItemInstance(itemProfile, item.Quantity, item.Durability);
        
        SharedStorageManager.Instance.GetItemFromStorage(itemInstance);
    }
}
