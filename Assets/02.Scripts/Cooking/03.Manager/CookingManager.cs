using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

public class CookingManager : NetworkBehaviourSingleton<CookingManager>
{
    private CookingPotInteractable _currentCookingPot;

    public Inventory FoodInventory = new Inventory(1);
    public Inventory IngredientInventory = new Inventory(2); // 로컬 아이템이고
    private Inventory _inputIngredientInventory;
    public List<Action> OnCookingSlotUpdated = new List<Action>(new Action[2]);
    public event Action<string, float> OnAlertMessage; // 문자열 알림용
    public event Action<ItemInstance> CookingFinished; // 결과 아이템 전체 전달용
    public event Action<ItemInstance> OnCompletedPopupStarted;
    
    public bool IsSpawned => Object != null && Object.IsValid;
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
        if (!_isCooking)
        {
            Debug.Log("요리가 진행 중이 아닙니다.");
            return;
        }
        _currentCookingPot.Rpc_EndCooking();
        _isCooking = false;
        
        if (p)
        {
            ProcessCookingResult();
        }
        else
        {
            ReturnRecipesToInventory();
            OnAlertMessage?.Invoke("요리가 취소되었습니다.", 1.2f);
            MasterAudio.PlaySound3DAtTransform("CookFail", _currentCookingPot.transform);
        }
    }
    
    public int TryCook()
    {
        int id1 = IngredientInventory.SlotList[0].ItemInstance.ID;
        int id2 = IngredientInventory.SlotList[1].ItemInstance.ID;
        
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
        
        // for (int i = 0; i < quantity; i++)
        // {
        //     var localPlayer = PlayerInfoManager.Instance.LocalPlayer;
        //     localPlayer.Skill.Publish(ESkillEventType.OnCook, new CookPayload(itemId, 1));
        // }
        
        UnifiedInventoryManager.Instance.AddItem(new ItemInstance(resultItem, quantity));
        RPC_BroadcastCookingResult(itemId);
        OnCompletedPopupStarted?.Invoke(new ItemInstance(resultItem));
        MasterAudio.PlaySound3DAtTransform("CookCompleted", _currentCookingPot.transform);
        MasterAudio.PlaySound3DAtTransform("CookSuccess", _currentCookingPot.transform);
    }
    
    private void TransferItemToInventory(ItemInstance itemInstance)
    {
        UnifiedInventoryManager.Instance.AddItem(itemInstance);
    }

    public void TryStartCook()
    {
        if (HasEmptySlot())
        {
            Debug.Log("[CookingManager] 빈 슬롯이 있어 요리를 시작할 수 없습니다.");
            return;
        }
        
        // 기본+강화무기/강화+강화무기는 요리 못하게 리턴
        
        bool isWeaponCombination
            = (IngredientInventory.SlotList[0].ItemInstance.ItemProfile.ItemDefinition.Type == EItemType.Weapon)
              && (IngredientInventory.SlotList[1].ItemInstance.ItemProfile.ItemDefinition.Type == EItemType.Weapon);

        if (isWeaponCombination)
        {
            OnAlertMessage?.Invoke("무기끼리는 요리할 수 없어요!", 1.2f);
            ReturnRecipesToInventory();
            return;
        }
        
        _currentCookingPot?.Rpc_StartCooking(Runner.LocalPlayer);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartCooking([RpcTarget] PlayerRef player)
    {
        _isCooking = true;
        OnAlertMessage?.Invoke("요리를 시작합니다! 재료들이 보글보글 끓고 있어요.", 3.8f);
        MasterAudio.PlaySound3DAtTransform("Cooking", _currentCookingPot.transform);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_CookingPotAlreadyUse([RpcTarget] PlayerRef player)
    {
        OnAlertMessage?.Invoke("다른 파티원이 이미 요리중입니다.", 1.2f);
        MasterAudio.PlaySound3DAtTransform("Fail", _currentCookingPot.transform);
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_BroadcastRecipeUnlockSync(int recipeID, RpcInfo info = default)
    {
        Debug.Log($"[CookingManager] RPC로 레시피 해금 상태만 동기화: recipeID={recipeID}");

        // RoomRecipeStateManager의 안전한 메서드를 통해 해금 처리
        if (RoomRecipeStateManager.Instance != null)
        {
            RoomRecipeStateManager.Instance.UnlockRecipeWithEvent(recipeID);

            // RecipeShopManager에 해금 알림 (구매 리스트에서 제거용)
            if (RecipeShopManager.Instance != null)
            {
                RecipeShopManager.Instance.OnRecipeUnlocked(recipeID);
            }
        }
        else
        {
            Debug.LogWarning("[CookingManager] RoomRecipeStateManager.Instance가 null입니다.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_BroadcastRecipePurchase(int recipeItemID, RpcInfo info = default)
    {
        Debug.Log($"[CookingManager] RPC로 레시피 구매 정보 동기화: recipeItemID={recipeItemID}");

        // 모든 클라이언트의 RecipeShopManager에서 구매 처리
        if (RecipeShopManager.Instance != null)
        {
            RecipeShopManager.Instance.OnRecipeItemPurchasedFromNetwork(recipeItemID);
        }
        else
        {
            Debug.LogWarning("[CookingManager] RecipeShopManager.Instance가 null입니다.");
        }
    }
}
