using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    private Inventory _inventory;
    public int InventorySize;

    private void Awake()
    {
        _inventory = new Inventory(InventorySize);
    }

    public ItemStack GetItemStack(int slotIndex)
    {
        return (_inventory.SlotList[slotIndex].ItemStack);
    }

    public bool TryPutItem(int slotIndex, ItemStack itemStack)
    {
        // 슬롯에 아이템을 넣을 수 있는지 확인하는 로직
        return true;
    }
    
    public bool IsEmptySlot(int slotIndex)
    {
        return (_inventory.SlotList[slotIndex].ItemStack == null);
    }
}
