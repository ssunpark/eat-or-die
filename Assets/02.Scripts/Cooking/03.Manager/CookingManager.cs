using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CookingManager : NetworkBehaviour
{
    public static CookingManager Instance { get; private set; }

    public Inventory IngredientInventory = new Inventory(2); // 로컬 아이템이고
    public List<Action> OnCookingSlotUpdated = new List<Action>(new Action[2]);
    
    public Inventory FoodInventory = new Inventory(1);
    public event Action OnCookOutputUpdated;
    
    private Inventory _inputIngredientInventory;
    
    // Networked 변수는 이름에 추가했으면 좋겠다
    [Networked] private PlayerRef _currentRequester { get; set; }
    [Networked] private NetworkBool _isCooking { get; set; }
    private float _cookTime = 4f;
    
    private float _t;
    public bool IsSpawned => Object != null && Object.IsValid; // Update에서 관여를 하는데 Networked변수는 Spawn이후에 접근이 가능함 IsSpawned
    
    private bool _amICooking;
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
        
        RPC_IsCookingCheck();
        
        if (_t >= _cookTime)
        {
            ProcessCookingResult();
        }
        else
        {
            ReturnRecipesToInventory();
        }
        
        _t = 0; // _t 초기화
        _amICooking = false;
    }
    
    // RPC가 _isCooking을 false로 만들어주는데 1프레임정도의 딜레이가 생겨서 1프레임도안 TryCook이 2번실행
    public int TryCook()
    {
        // if (IngredientInventory.SlotList[0].IsEmpty || IngredientInventory.SlotList[1].IsEmpty)
        // {
        //     Debug.LogWarning("[TryCook] 재료 슬롯이 비어 있습니다.");
        //     return -1;
        // }
        //
        // if (IngredientInventory.SlotList[0].Item == null || IngredientInventory.SlotList[1].Item == null)
        // {
        //     Debug.LogError("[TryCook] 슬롯 아이템이 null입니다.");
        //     return -1;
        // }
        
        int id1 = IngredientInventory.SlotList[0].Item.ID;
        int id2 = IngredientInventory.SlotList[1].Item.ID;
        
       
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
        // if (resultItemId == -1)
        // {
        //     return;
        // }
        ConsumeInputIngredients();
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
        var resultItem = ItemManager.Instance.GetItem(itemId);
        if (resultItem == null)
        {
            Debug.Log($"[CookingManager] 결과 아이템 데이터가 없습니다. ID: {itemId}");
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
    
    private void Update()
    {
        // 네트워크 연결 이후 작동하게 하기 위함
        if(!IsSpawned) return;
        
        if (_isCooking && _currentRequester == Runner.LocalPlayer && !_amICooking)
        {
            _t += Time.deltaTime;
            
            if (_t >= _cookTime)
            {
                _amICooking = true;
                OnCookingCompleted();
            }
        }
    }
    // 요리 진행 _isCooking만 트루로 바꾸고 나머지는 로컬에서 진행
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestStartCook(PlayerRef player)
    {
        if (_isCooking)
        {
            Debug.Log("[CookingManager] 서버에서 이미 요리 중입니다.");
            return;
        }
        
        //상태들만 바꿈
        _isCooking = true;
        _currentRequester = player;
    }
    
    // 패널에서 이 코드 실행
    public void TryStartCookRPC()
    {
        if (_isCooking)
        {
            Debug.Log("[CookingManager] 이미 요리 중입니다.");
            // ReturnRecipesToInventory(); // 만약 이미 요리 중일때 인벤토리로 보내고 싶은 경우.
            return;
        }

        if (HasEmptySlot())
        {
            Debug.Log("[CookingManager] 빈 슬롯이 있어 요리를 시작할 수 없습니다.");
            return;
        }

        RPC_RequestStartCook(Runner.LocalPlayer); // 서버에게 요리 시작 요청
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_IsCookingCheck()
    {
        _isCooking = false;
    }
}
