using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CookingManager : NetworkBehaviour
{
    public static CookingManager Instance { get; private set; }

    public Inventory IngredientInventory = new Inventory(2); // 로컬 아이템이고
    public List<Action> OnCookingSlotUpdated = new List<Action>(new Action[2]);
    
    public Inventory FoodInventory = new Inventory(1);
    public Action OnCookOutputUpdated;
    
    private Inventory _inputIngredientInventory;
    [Networked] private PlayerRef _currentRequester { get; set; }
    [Networked] private float _t { get; set; }
    [Networked] private bool _isCooking { get; set; }
    private float _cookTime = 4f;

    private int _id1;
    private int _id2;
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
            var itemInSlot = IngredientInventory.PopItemInSlot(slotIndex);
            if (itemInSlot == null) return;
            HandEntity.Instance.PickUpItem(itemInSlot);
        }
        else
        {
            HandEntity.Instance.PickUpItem(IngredientInventory.PutItemInSlot(slotIndex, HandEntity.Instance.Item));
        }
        OnCookingSlotUpdated[slotIndex]?.Invoke();
    }

    public void OnClickMouseRight(int slotIndex)
    {

        if (IngredientInventory.SlotList[slotIndex].IsEmpty) return;

        if (HandEntity.Instance.IsHandEmpty)
        {
            HandEntity.Instance.PickUpItem(IngredientInventory.PopSingleItemInSlot(slotIndex));
        }
        else
        {
            if (HandEntity.Instance.Item.ID == IngredientInventory.SlotList[slotIndex].Item.ID)
            {
                var itemInSlot = IngredientInventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInSlot))
                {
                    IngredientInventory.SlotList[slotIndex].Item.TryAdd(itemInSlot.Quantity);
                }
            }
            else
            {
                var temp = IngredientInventory.PopItemInSlot(slotIndex);
                IngredientInventory.PutItemInSlot(slotIndex, HandEntity.Instance.Item);
                HandEntity.Instance.PickUpItem(temp);
            }
        }
        OnCookingSlotUpdated[slotIndex]?.Invoke();
    }
    
    private bool HasEmptySlot()
    {
        return IngredientInventory.SlotList.Exists(slot => slot.IsEmpty);
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
        
        // 이 로직을 RecipeManager로 빼서 거기서 레시피 습득 여부까지 판단하도록
        foreach (var recipe in RecipeManager.Instance.RecipeList)
        {
            if ((recipe.Ingredient1ID == _id1 && recipe.Ingredient2ID == _id2) ||
                (recipe.Ingredient1ID == _id2 && recipe.Ingredient2ID == _id1))
            {
                return recipe.ResultID;
            }
        }
        return 200044; // 애매한 요리 ID
    }
    
    public void ProcessCookingResult()
    {
        int resultItemId = TryCook();
    
        // ConsumeInputIngredients();
        GiveItemToInventory(resultItemId);
        ReturnRecipesToInventory();
        OnCookOutputUpdated?.Invoke();
    }
    
    public void ReturnRecipesToInventory()
    {
        foreach (var slot in IngredientInventory.SlotList)
        {
            if (!slot.IsEmpty)
            {
                TransferItemToInventory(slot.Item);
                slot.RemoveItem();
            }
        }
        OnCookingSlotUpdated.ForEach(action => action?.Invoke());
    }
    
    public void ConsumeInputIngredients()
    {
        foreach (var slot in IngredientInventory.SlotList)
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
        Debug.Log($"{_currentRequester}!~!!!!!!@@!@!@!!!!!!!!!!!!!!!!!!!!");
        // 요청자 객체 찾기
        NetworkObject playerObj = Runner.GetPlayerObject(_currentRequester);
        if (playerObj == null)
        {
            Debug.Log($"[CookingManager] 요청자 PlayerRef에 대한 NetworkObject를 찾을 수 없습니다: {_currentRequester}");
            return;
        }

        InventoryManager.Instance.PickItemFromGround(new Item(resultItem, 1));
        InventoryManager.Instance.OnInventoryUpdated?.Invoke();
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
    public void RPC_StartCook(int ingredient1Id, int ingredient2Id, RpcInfo info = default)
    {
        Debug.Log(">>> 요리 요청 도착");
        _id1 = ingredient1Id;
        _id2 = ingredient2Id;

        if (_isCooking)
        {
            Debug.Log("요리가 이미 진행 중입니다.");
            return;
        }

        if (ingredient1Id == -1 || ingredient2Id == -1)
        {
            Debug.Log("빈 슬롯이 있어 요리를 시작할 수 없습니다.");
            return;
        }

        _isCooking = true;
        _t = 0f;
        _currentRequester = info.Source;
        
    }

    public void TryStartCookRPC()
    {
        if (_isCooking)
        {
            Debug.Log("[CookingManager] 이미 요리 중입니다.");
            return;
        }

        if (HasEmptySlot())
        {
            Debug.Log("[CookingManager] 빈 슬롯이 있어 요리를 시작할 수 없습니다.");
            return;
        }

        // 두 슬롯에서 재료 ID 추출
        int id1 = IngredientInventory.SlotList[0].Item.ID;
        int id2 = IngredientInventory.SlotList[1].Item.ID;

        // 둘 중 하나라도 잘못된 아이디면 중단
        if (id1 == -1 || id2 == -1)
        {
            Debug.Log("[CookingManager] 유효하지 않은 재료가 있습니다.");
            return;
        }

        // RPC 호출 (서버에게 요리 시작 요청)
        RPC_StartCook(id1, id2);
    }

}
