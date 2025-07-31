using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CookingManager : NetworkBehaviour
{
    public static CookingManager Instance { get; private set; }

    public Inventory Inventory = new Inventory(2);
    public List<Action> OnCookingSlotUpdated = new List<Action>(new Action[2]);
    
    public Inventory FoodInventory = new Inventory(1);
    public Action OnCookOutputUpdated;
    
    [Networked] private PlayerRef _currentRequester { get; set; }
    [Networked] private float _t { get; set; }
    [Networked] private bool _isCooking { get; set; }
    private float _cookTime = 4f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
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
        Debug.Log("OnCookingCompleted 진입!!!");
        
        if (!_isCooking)
        {
            Debug.Log("요리가 진행 중이 아닙니다.");
            return;
        }
        _isCooking = false;
        
        // InputReader.playerControllerInputBlocked = false;
        
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
    
        // 이 로직을 RecipeManager로 빼서 거기서 레시피 습득 여부까지 판단하도록
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
        // var resultItem = ItemManager.Instance.GetItem(itemId);
        // if (resultItem == null)
        // {
        //     Debug.LogError($"[CookingPanelManager] 결과 아이템 데이터가 없습니다. ID: {itemId}");
        //     return;
        // }
        //
        // InventoryManager.Instance.PickItemFromGround(new Item(resultItem, 1)); // 나중에 한번에 여러개 만드는거 생기면 1을 바꾸시면 됩니다
        // InventoryManager.Instance.OnInventoryUpdated?.Invoke();
        var resultItem = ItemManager.Instance.GetItem(itemId);
        if (resultItem == null)
        {
            Debug.Log($"[CookingManager] 결과 아이템 데이터가 없습니다. ID: {itemId}");
            return;
        }

        // 요청자 객체 찾기
        NetworkObject playerObj = Runner.GetPlayerObject(_currentRequester);
        if (playerObj == null)
        {
            Debug.Log($"[CookingManager] 요청자 PlayerRef에 대한 NetworkObject를 찾을 수 없습니다: {_currentRequester}");
            return;
        }

        var inventoryManager = playerObj.GetComponent<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.Log("[CookingManager] 요청자의 InventoryManager가 존재하지 않습니다.");
            return;
        }

        inventoryManager.PickItemFromGround(new Item(resultItem, 1));
        inventoryManager.OnInventoryUpdated?.Invoke();
    }
    
    private void TransferItemToInventory(Item item)
    {
        InventoryManager.Instance.PickItemFromGround(item);
        InventoryManager.Instance.OnInventoryUpdated?.Invoke();
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!_isCooking) return;
        _t += Runner.DeltaTime;
        if (_t >= _cookTime)
        {
            // 플레이어 연결 미완료여서 임시로 플레이어와 상호작용없이 요리 완료
            OnCookingCompleted();
            //Room.Instance.LocalPlayer.GetComponent<PlayerStateMachine>().RequestChangeState(EPlayerState.Idle);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    internal void RPC_StartCook(PlayerRef requester)
    {
        if (_isCooking)
        {
            Debug.Log("요리가 이미 진행 중입니다.");
            return;
        }
        
        if (HasEmptySlot())
        {
            Debug.Log("빈 슬롯이 있어 요리를 시작할 수 없습니다.");
            return; // 빈 슬롯이면 return. 쿠킹 패널만 닫힘.
        }
        // Room.Instance.LocalPlayer.GetComponent<PlayerStateMachine>().RequestChangeState(EPlayerState.Cooking);
        _t = 0;
        _isCooking = true; // rpc로 변환
        _currentRequester = requester;
    }
    
}
