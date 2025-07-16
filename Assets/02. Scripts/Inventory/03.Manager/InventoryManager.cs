using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    private Inventory _inventory;
    public int InventorySize;

    private void Awake()
    {
        _inventory = new Inventory(InventorySize);
    }

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            ItemStack itemInSlot = _inventory.PopItemInSlot(slotIndex);
            if (itemInSlot == null) return;
            
            HandEntity.Instance.PickUpItem(itemInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(_inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemStack));
        }
    }
    
    public void OnClickMouseRight(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            // 손이 비어있을 때 아이템 스택 한개를 전달하는 로직
        }
        else
        {
            // 1. 슬롯이 비어있을 때는 한개씩 내려놓기
            // 2. 슬롯에 아이템이 있는데 손에 있는 아이템과 종류가 같을 때는 한개씩 더 집기
            // 3. 슬롯에 아이템이 있는데 손에 있는 아이템과 종류가 다를 때는 슬롯의 아이템 전체와 손의 아이템 전체를 스왑
        }
        // UI Refresh 이벤트 발생시켜보자고요
    }

    public ItemStack PickItemFromGround(ItemStack itemStack)
    {
        return _inventory.PickItemFromGround(itemStack);
    }
}
