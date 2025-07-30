using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingManager : BehaviourSingleton<CookingManager>
{
    public Inventory Inventory = new Inventory(2);
    public List<Action> OnCookingSlotUpdated = new List<Action>(new Action[2]);
    
    public Inventory FoodInventory = new Inventory(1);
    public Action OnCookOutputUpdated;
    public void OnClickMouseLeft(int slotIndex)
    {
        if (HandEntity.Instance.IsHandEmpty)
        {
            var itemInSlot = Inventory.PopItemInSlot(slotIndex);
            if (itemInSlot == null) return;
            HandEntity.Instance.PickUpItem(itemInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(Inventory.PutItemInSlot(slotIndex, HandEntity.Instance.Item));
        }
        OnCookingSlotUpdated[slotIndex]?.Invoke();
    }

    public void OnClickMouseRight(int slotIndex)
    {
        if (Inventory.SlotList[slotIndex].IsEmpty) return;

        if (HandEntity.Instance.IsHandEmpty)
        {
            HandEntity.Instance.PickUpItem(Inventory.PopSingleItemInSlot(slotIndex));
        }
        else
        {
            if (HandEntity.Instance.Item.ID == Inventory.SlotList[slotIndex].Item.ID)
            {
                var itemInSlot = Inventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInSlot))
                {
                    Inventory.SlotList[slotIndex].Item.TryAdd(itemInSlot.Quantity);
                }
            }
            else
            {
                var temp = Inventory.PopItemInSlot(slotIndex);
                Inventory.PutItemInSlot(slotIndex, HandEntity.Instance.Item);
                HandEntity.Instance.PickUpItem(temp);
            }
        }
        OnCookingSlotUpdated[slotIndex]?.Invoke();
    }
    
    private bool HasEmptySlot()
    {
        return Inventory.SlotList.Exists(slot => slot.IsEmpty);
    }
    
    public void OnCookingCompleted()
    {
        if (!_isCooking)
        {
            Debug.LogWarning("요리가 진행 중이 아닙니다.");
            return;
        }
        _isCooking = false;
        if (_t>=_cookTime)
        {
            ProcessCookingResult();
        }
        else
        {
            ReturnRecipesToInventory();
        }
    }
    
    public int TryCook()
    {
        int id1 = Inventory.SlotList[0].Item.ID;
        int id2 = Inventory.SlotList[1].Item.ID;
    
        foreach (var recipe in RecipeManager.Instance.RecipeList)
        {
            if ((recipe.Ingredient1ID == id1 && recipe.Ingredient2ID == id2) ||
                (recipe.Ingredient1ID == id2 && recipe.Ingredient2ID == id1))
            {
                return recipe.ResultID;
            }
        }
        return 200044; // 애매한 요리 ID
    }
    
    public void ProcessCookingResult()
    {
        int resultItemId = TryCook();
    
        ConsumeInputIngredients();
        GiveItemToInventory(resultItemId);
        ReturnRecipesToInventory();
        OnCookOutputUpdated?.Invoke();
    }
    
    public void ReturnRecipesToInventory()
    {
        foreach (var slot in Inventory.SlotList)
        {
            if (!slot.IsEmpty)
            {
                TransferItemToInventory(slot.Item);
                slot.RemoveItem();
            }
        }
        OnCookingSlotUpdated.ForEach(action => action?.Invoke());
    }
    
    private void ConsumeInputIngredients()
    {
        foreach (var slot in Inventory.SlotList)
        {
            slot.UseItem();
        }
        OnCookingSlotUpdated.ForEach(action => action?.Invoke());
    }
    
    private void GiveItemToInventory(int itemId)
    {
        var resultItem = ItemManager.Instance.GetItem(itemId);
        if (resultItem == null)
        {
            Debug.LogError($"[CookingPanelManager] 결과 아이템 데이터가 없습니다. ID: {itemId}");
            return;
        }
    
        InventoryManager.Instance.PickItemFromGround(new Item(resultItem, 1)); // 나중에 한번에 여러개 만드는거 생기면 1을 바꾸시면 됩니다
        InventoryManager.Instance.OnInventoryUpdated?.Invoke();
    }
    
    private void TransferItemToInventory(Item item)
    {
        InventoryManager.Instance.PickItemFromGround(item);
        InventoryManager.Instance.OnInventoryUpdated?.Invoke();
    }
    private float _t;
    private float _cookTime = 4f;
    private bool _isCooking;
    
    private void Update()
    {
        if (!_isCooking) return;
        _t += Time.deltaTime;
        if (_t >= _cookTime)
        {
            // 플레이어 연결 미완료여서 임시로 플레이어와 상호작용없이 요리 완료
            OnCookingCompleted();
            //Room.Instance.LocalPlayer.GetComponent<PlayerStateMachine>().RequestChangeState(EPlayerState.Idle);
        }
    }
    
    internal void StartCook()
    {
        if (HasEmptySlot()) return; // 빈 슬롯이면 return. 쿠킹 패널만 닫힘.
        // Room.Instance.LocalPlayer.GetComponent<PlayerStateMachine>().RequestChangeState(EPlayerState.Cooking);
        _t = 0;
        _isCooking = true; // rpc로 변환
    }
    
}
