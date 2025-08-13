using System;
using System.Linq;
using Fusion;

public class RoomRecipeStateManager : NetworkBehaviourSingleton<RoomRecipeStateManager>
{
    public event Action<Recipe> OnRecipeUnlocked;
    public event Action<int> OnIngredientUnlocked;

    private void OnEnable()
    {
        UnifiedInventoryManager.Instance.OnItemAcquired += HandleIngredientDiscovered;
        CookingManager.Instance.CookingFinished += HandleCookingFinished;

    }
    
    private void OnDisable()
    {
        if (UnifiedInventoryManager.Instance != null)
        {
            UnifiedInventoryManager.Instance.OnItemAcquired -= HandleIngredientDiscovered;
        }

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.CookingFinished -= HandleCookingFinished;
        }
    }

    public bool IsUnlockedIngredients(int ingredientID)
    {
        return RoomInfoManager.Instance.CurrentRoomInfo.KnownIngredients.Contains(ingredientID);
    }

    public bool IsUnlockedRecipes(int recipeID)
    {
        return RoomInfoManager.Instance.CurrentRoomInfo.KnownRecipes.Contains(recipeID);
    }

    public bool TryUnlockIngredient(int ingredientID)
    {
        // 중복 해금 방지 로직을 다시 활성화하는 것이 좋습니다.
        if (IsUnlockedIngredients(ingredientID))
        {
            return false;
        }

        var success = RoomInfoManager.Instance.CurrentRoomInfo.AddIngredient(ingredientID);
        if (success)
        {
            RoomInfoManager.Instance.Save();

            // 로컬 이벤트를 직접 호출하는 대신, 모든 클라이언트에게 결과를 알리는 RPC를 호출합니다.
            RPC_NotifyIngredientUnlocked(ingredientID);
            // // 저장이 성공했을 때, 이 메서드가 직접 이벤트를 발생시킵니다.
            // OnIngredientUnlocked?.Invoke(ingredientID);
        }

        return success;
    }

    // 결과를 모든 클라이언트에게 전파하는 RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_NotifyIngredientUnlocked(int ingredientID)
    {
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
        // 이 RPC는 서버(State Authority)에서만 실행됩니다.
        // 서버는 전달받은 ID로 실제 해금 로직을 실행합니다.
        TryUnlockIngredient(ingredientID);
    }
}
