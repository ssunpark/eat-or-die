using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class RoomRecipeStateManager : NetworkBehaviourSingleton<RoomRecipeStateManager>
{
    public event Action<Recipe> OnRecipeUnlocked;
    public event Action<int> OnIngredientUnlocked;
    // private bool _isInitialized;

    private void Start()
    {
        // 데이터가 준비되었다는 신호를 기다림
        // RoomInfoManager.Instance.OnCurrentRoomInfoUpdated += Initialize;
        Initialize();
    }

    // RoomInfoManager의 데이터 동기화가 완료되면 호출될 초기화 메서드
    private void Initialize()
    {
        // 이미 초기화되었다면 중복 실행 방지
        // if (_isInitialized)
        // {
        //     return;
        // }

        Debug.Log("RoomInfo 동기화 완료! RoomRecipeStateManager 로직을 활성화합니다.");

        // 이제 CurrentRoomInfo가 안전하므로, 게임 로직 관련 이벤트를 구독
        UnifiedInventoryManager.Instance.OnItemAcquired += HandleIngredientDiscovered;
        CookingManager.Instance.CookingFinished += HandleCookingFinished;

        // _isInitialized = true;
        //
        // // 초기화가 끝났으므로 더 이상 필요 없는 이벤트 구독 해제
        // RoomInfoManager.Instance.OnCurrentRoomInfoUpdated -= Initialize;
    }

    private void OnDisable()
    {
        // 안전하게 모든 이벤트 구독 해제
        if (RoomInfoManager.Instance != null)
        {
            RoomInfoManager.Instance.OnCurrentRoomInfoUpdated -= Initialize;
        }
        if (UnifiedInventoryManager.Instance != null)
        {
            UnifiedInventoryManager.Instance.OnItemAcquired -= HandleIngredientDiscovered;
        }
        // ...
    }


    // private void OnEnable()
    // {
    //     UnifiedInventoryManager.Instance.OnItemAcquired += HandleIngredientDiscovered;
    //     CookingManager.Instance.CookingFinished += HandleCookingFinished;
    // }
    //
    // private void OnDisable()
    // {
    //     if (UnifiedInventoryManager.Instance != null)
    //     {
    //         UnifiedInventoryManager.Instance.OnItemAcquired -= HandleIngredientDiscovered;
    //     }
    //     
    //     if (CookingManager.Instance != null)
    //     {
    //         CookingManager.Instance.CookingFinished -= HandleCookingFinished;
    //     }
    // }

    public bool IsUnlockedIngredients(int ingredientID)
    {
        return RoomInfoManager.Instance.CurrentRoomInfo.KnownIngredients.Contains(ingredientID);
    }

    public bool IsUnlockedRecipes(int recipeID)
    {
        return RoomInfoManager.Instance.CurrentRoomInfo.KnownRecipes.Contains(recipeID);
    }

    public async UniTask TryUnlockIngredient(int ingredientID)
    {
        // 중복 해금 방지 로직을 다시 활성화하는 것이 좋습니다.
        if (IsUnlockedIngredients(ingredientID))
        {
            return;
        }

        var success = RoomInfoManager.Instance.CurrentRoomInfo.AddIngredient(ingredientID);
        if (success && HasStateAuthority)
        {
            await RoomInfoManager.Instance.Save();

            // 로컬 이벤트를 직접 호출하는 대신, 모든 클라이언트에게 결과를 알리는 RPC를 호출합니다.
            RPC_NotifyIngredientUnlocked(ingredientID);
            // // 저장이 성공했을 때, 이 메서드가 직접 이벤트를 발생시킵니다.
            // OnIngredientUnlocked?.Invoke(ingredientID);
        }

        return;
    }

    // 결과를 모든 클라이언트에게 전파하는 RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyIngredientUnlocked(int ingredientID)
    {
        Debug.Log("재료 UI 업데이트!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        // 이 RPC는 모든 클라이언트에서 실행됩니다.
        // 여기서 로컬 이벤트를 발생시키면, 모든 클라이언트의 UI가 갱신됩니다.
        OnIngredientUnlocked?.Invoke(ingredientID);
    }

    public bool TryUnlockRecipe(int recipeID)
    {
        // if (IsUnlocked(recipeID)) return false;

        bool success = RoomInfoManager.Instance.CurrentRoomInfo.AddRecipe(recipeID);
        if (success)
        {
            RoomInfoManager.Instance.Save();
        }
        return success;
    }

    private void HandleIngredientDiscovered(ItemInstance acquiredItem)
    {
        Debug.Log("아이템 주움!!!!!!!!!!!: " + acquiredItem.ID);
        if (acquiredItem == null)
        {
            return;
        }

        var itemProfile = ItemManager.Instance.GetItem(acquiredItem.ID);
        if (itemProfile == null || !itemProfile.ItemDefinition.IsIngredient)
        {
            return;
        }

        // 받은 ItemInstance에서 ID를 꺼내 TryUnlockIngredient에 전달합니다.
        RPC_RequestIngredientUnlock(acquiredItem.ID);
    }
    
    private void HandleCookingFinished(ItemInstance cookedItem)
    {
        
        var recipe = RecipeManager.Instance.RecipeList.Find(r => r.ResultID == cookedItem.ID);
        if (recipe == null) return;

        if (TryUnlockRecipe(recipe.ID))
        {
            OnRecipeUnlocked?.Invoke(recipe);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RequestIngredientUnlock(int ingredientID)
    {
        Debug.Log("RPC_RequestIngredientUnlock 호출: " + ingredientID);
        // 이 RPC는 서버(State Authority)에서만 실행됩니다.
        // 서버는 전달받은 ID로 실제 해금 로직을 실행합니다.
        TryUnlockIngredient(ingredientID);
    }
}
