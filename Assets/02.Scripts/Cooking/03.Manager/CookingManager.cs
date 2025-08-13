using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CookingManager : NetworkBehaviourSingleton<CookingManager>
{
    private CookingPotInteractable _currentCookingPot;

    public Inventory FoodInventory = new Inventory(1);
    public Inventory IngredientInventory = new Inventory(2); // 로컬 아이템이고
    private Inventory _inputIngredientInventory;
    public List<Action> OnCookingSlotUpdated = new List<Action>(new Action[2]);

    public Action OnCookOutputUpdated;
    public event Action<string> OnAlertMessage; // 문자열 알림용
    public event Action<ItemInstance> CookingFinished; // 결과 아이템 전체 전달용
    public event Action<ItemInstance> OnCompletedPopupStarted;
    public event Action OnItemAdded;
    
    public bool IsSpawned => Object != null && Object.IsValid; // Update에서 관여를 하는데 Networked변수는 Spawn이후에 접근이 가능함 IsSpawned
    private bool _isCooking;

    public void SetCurrentCookingPot(CookingPotInteractable cookingPot)
    {
        _currentCookingPot = cookingPot;
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
            if (!HandEntity.Instance.GetItem().ItemProfile.ItemDefinition.IsIngredient) return;
            HandEntity.Instance.PickUpItem(IngredientInventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance));
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
            if (HandEntity.Instance.ItemInstance.ID == IngredientInventory.SlotList[slotIndex].ItemInstance.ID)
            {
                var itemInSlot = IngredientInventory.PopSingleItemInSlot(slotIndex);
                if (!HandEntity.Instance.TryAddItem(itemInSlot))
                {
                    IngredientInventory.SlotList[slotIndex].ItemInstance.TryAdd(itemInSlot.Quantity);
                }
            }
            else
            {
                if (!HandEntity.Instance.GetItem().ItemProfile.ItemDefinition.IsIngredient) return;
                var temp = IngredientInventory.PopItemInSlot(slotIndex);
                IngredientInventory.PutItemInSlot(slotIndex, HandEntity.Instance.ItemInstance);
                HandEntity.Instance.PickUpItem(temp);
            }
        }
        OnCookingSlotUpdated[slotIndex]?.Invoke();
    }

    public bool HasEmptySlot()
    {
        return IngredientInventory.SlotList.Exists(slot => slot.IsEmpty);
    }
    
    public void OnCookingCompleted(bool p)
    {
        // 실제로는 PlayerState의 OnEndState 메서드 내부에서 이 함수가 호출됨
        if (!_isCooking)
        {
            Debug.Log("요리가 진행 중이 아닙니다.");
            return;
        }
        
        // RPC_IsCookingCheck();
        _currentCookingPot.Rpc_EndCooking();
        _isCooking = false;
        
        if (p)
        {
            ProcessCookingResult();
        }
        else
        {
            ReturnRecipesToInventory();
            OnAlertMessage?.Invoke("요리가 취소되었습니다.");
        }
        
        // _amICooking = false;
    }
    
    // RPC가 _isCooking을 false로 만들어주는데 1프레임정도의 딜레이가 생겨서 1프레임도안 TryCook이 2번실행
    public int TryCook()
    {
        int id1 = IngredientInventory.SlotList[0].ItemInstance.ID;
        int id2 = IngredientInventory.SlotList[1].ItemInstance.ID;
        
       
        // 이 로직을 RecipeManager로 빼서 거기서 레시피 습득 여부까지 판단하도록
        foreach (var recipe in RecipeManager.Instance.RecipeList)
        {
            if ((recipe.Ingredient1ID == id1 && recipe.Ingredient2ID == id2) ||
                (recipe.Ingredient1ID == id2 && recipe.Ingredient2ID == id1))
            {
                return recipe.ResultID;
            }
        }
        
        Dictionary<int, int> specialIngredientResultMap = new Dictionary<int, int>
        {
            { 200013, 200121 }, // 강철 -> 단단한 요리
            { 200028, 200122 } // 드래곤 고기 -> 드래곤 스테이크
            // 추가 가능
        };

        HashSet<int> inputSet = new HashSet<int> { id1, id2 };
        foreach (int id in inputSet)
        {
            if (specialIngredientResultMap.TryGetValue(id, out int result))
            {
                return result;
            }
        }
        
        return 200120; // 애매한 요리 ID
    }
    
    public void ProcessCookingResult()
    {
        var quantityToCook = Mathf.Min(
            IngredientInventory.SlotList[0].ItemInstance.Quantity,
            IngredientInventory.SlotList[1].ItemInstance.Quantity
        );
        
        int resultItemId = TryCook();
        ConsumeInputIngredients(quantityToCook);
        GiveItemToInventory(resultItemId, quantityToCook);
        ReturnRecipesToInventory();
        // OnCookOutputUpdated?.Invoke();
    }
    
    public void ReturnRecipesToInventory()
    {
        foreach (var slot in IngredientInventory.SlotList)
        {
            if (!slot.IsEmpty)
            {
                TransferItemToInventory(slot.ItemInstance);
                slot.RemoveItem();
            }
        }
        OnCookingSlotUpdated.ForEach(action => action?.Invoke());
    }

    public void ConsumeInputIngredients(int quantity)
    {
        foreach (var slot in IngredientInventory.SlotList)
        {
            slot.UseItem(quantity);
        }
        OnCookingSlotUpdated.ForEach(action => action?.Invoke());
    }


    private void GiveItemToInventory(int itemId, int quantity)
    {
        var resultItem = ItemManager.Instance.GetItem(itemId);
        if (resultItem == null)
        {
            Debug.Log($"[CookingManager] 결과 아이템 데이터가 없습니다. ID: {itemId}");
            return;
        }

        // InventoryManager.Instance.AddItemToInventory(new ItemInstance(resultItem, quantity));
        var localPlayer = PlayerInfoManager.Instance.LocalPlayer;
        localPlayer.Skill.Publish(ESkillEventType.OnCook, new CookPayload(itemId));
        UnifiedInventoryManager.Instance.AddItem(new ItemInstance(resultItem, quantity));
        // InventoryManager.Instance.OnInventoryUpdated?.Invoke();
        // CookingFinished?.Invoke(new ItemInstance(resultItem, 1));
        RPC_BroadcastCookingResult(itemId);
        OnCompletedPopupStarted?.Invoke(new ItemInstance(resultItem, 1));
        OnItemAdded?.Invoke();
        
    }
    
    private void TransferItemToInventory(ItemInstance itemInstance)
    {
        UnifiedInventoryManager.Instance.AddItem(itemInstance);
        // InventoryManager.Instance.OnInventoryUpdated?.Invoke();
    }

    public void TryStartCook()
    {
        if (HasEmptySlot())
        {
            Debug.Log("[CookingManager] 빈 슬롯이 있어 요리를 시작할 수 없습니다.");
            return;
        }

        _currentCookingPot?.Rpc_StartCooking(Runner.LocalPlayer);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartCooking([RpcTarget] PlayerRef player)
    {
        _isCooking = true;
        // FusionInputProvider.PlayerControllers[player].RequestState(EPlayerState.Cooking);
        OnAlertMessage?.Invoke(("요리를 시작합니다! 재료들이 보글보글 끓고 있어요."));
        // Room.Instance.LocalPlayer.GetComponent<Player>().RequestState(EPlayerState.Cooking);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_CookingPotAlreadyUse([RpcTarget] PlayerRef player)
    {
        OnAlertMessage?.Invoke("다른 파티원이 이미 요리중입니다.");
        
        // 만약 재료를 다시 인벤토리로 보내고 싶으면 
        // ReturnRecipesToInventory();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_IsCookingCheck()
    {
        _isCooking = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_BroadcastCookingResult(int resultItemId, RpcInfo info = default)
    {
        var resultItem = ItemManager.Instance.GetItem(resultItemId);
        var itemInstance = new ItemInstance(resultItem, 1);

        CookingFinished?.Invoke(itemInstance); // 레시피를 업데이트 시키는 이벤트
    }

    private void CheckOutput()
    {
    }
}
