using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingPanelManager : BehaviourSingleton<CookingPanelManager>
{
    // 드래그앤드랍, 클릭
    public Inventory Inventory = new Inventory(2);
    public List<Action> OnCookingSlotUpdated = new List<Action>(new Action[2]);

    public CookOutputSlotUI CookOutputSlotUI;
    public Inventory FoodInventory = new Inventory(1);
    
    public Action OnCookOutputUpdated; // 결과 슬롯 전용 이벤트
    
    // 좌클릭으로 아이템 드래그 처리
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
        OnCookingSlotUpdated[slotIndex]?.Invoke();
    }
    
    // 우클릭으로 아이템 한 개만 꺼내기 / 교환
    public void OnClickMouseRight(int slotIndex)
    {
        if (Inventory.SlotList[slotIndex].IsEmpty) return;
        
        if (HandEntity.Instance.IsHandEmpty)
        {
            HandEntity.Instance.PickUpItem(Inventory.PopSingleItemInSlot(slotIndex));
        }
        else
        {
            if (HandEntity.Instance.ItemStack.ID == Inventory.SlotList[slotIndex].ItemStack.ID)
            {
                ItemStack itemInSlot = Inventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInSlot))
                {
                    Inventory.SlotList[slotIndex].ItemStack.TryAdd(itemInSlot.Quantity);
                }
            }
            else
            {
                ItemStack temp = Inventory.PopItemInSlot(slotIndex);
                Inventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemStack);
                HandEntity.Instance.PickUpItem(temp);
            }
        }

        OnCookingSlotUpdated[slotIndex]?.Invoke();
    }

    public bool TryGetRecipeResult()
    {
        return false;
    }
    
    private bool HasEmptySlot()
    {
        foreach (var slot in Inventory.SlotList)
        {
            if (slot.IsEmpty)
            {
                return true; // 비어있는 슬롯이 있다
            }
        }
        return false; // 모두 차 있음
    }
    
    // 슬롯 2개가 비었는지 확인 후 레시피 매칭
    public int TryCook()
    {
        if (HasEmptySlot())
        {
            return -1;
        }

        int id1 = Inventory.SlotList[0].ItemStack.ID;
        int id2 = Inventory.SlotList[1].ItemStack.ID;

        foreach (RecipeCSVData recipe in FoodCSVDataManager.Instance.RecipeCSVDataList)
        {
            bool isMatch = (recipe.Ingredient1ID == id1 && recipe.Ingredient2ID == id2) ||
                           (recipe.Ingredient1ID == id2 && recipe.Ingredient2ID == id1);

            if (isMatch)
            {
                return recipe.ResultID; //recipe.ID
            }
        }

        // 일치하는 레시피가 없는 경우
        return -1;
    }

    // 조합 성공 시 재료 차감 + 결과 아이템 생성
    public void TryCookAndCreateItem()
    {
        int resultItemId = TryCook();
        if (resultItemId == -1)
        {
            Debug.Log("조합 실패");
            return;
        }

        foreach (var slot in Inventory.SlotList)
        {
            slot.UseItem();
        }

        for (int i = 0; i < OnCookingSlotUpdated.Count; i++)
        {
            OnCookingSlotUpdated[i]?.Invoke();
        }

        AItem resultItem = ItemManager.Instance.GetItem(resultItemId);
        FoodInventory.SlotList[0].AddItem(new ItemStack(resultItemId, resultItem.ItemData.MaxQuantity,1));
        
        OnCookOutputUpdated?.Invoke(); // 결과 슬롯 UI 갱신
    }
}
