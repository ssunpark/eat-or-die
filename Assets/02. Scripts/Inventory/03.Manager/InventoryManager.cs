using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : BehaviourSingleton<InventoryManager>
{
    public Inventory Inventory;
    public int InventorySize;
    
    public List<Action> OnSlotUpdated;
    public Action OnInventoryUpdated;

    private void Awake()
    {
        Inventory = new Inventory(InventorySize);
        OnSlotUpdated = new List<Action>(new Action[InventorySize]);
    }

    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            ItemStack itemInSlot = Inventory.PopItemInSlot(slotIndex);
            if (itemInSlot == null) return;
            
            HandEntity.Instance.PickUpItem(itemInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(Inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemStack));
        }
        OnSlotUpdated[slotIndex]?.Invoke();
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
        ItemStack remain = Inventory.PickItemFromGround(itemStack);

        OnInventoryUpdated?.Invoke();
        
        return remain;
    }
}
