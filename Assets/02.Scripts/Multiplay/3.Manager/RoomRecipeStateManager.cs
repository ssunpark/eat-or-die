using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class RoomRecipeStateManager : NetworkBehaviourSingleton<RoomRecipeStateManager>
{
    public event Action<Recipe> OnRecipeUnlocked;
    public event Action<int> OnIngredientUnlocked;

    private void Start()
    {
        UnifiedInventoryManager.Instance.OnItemAcquired += HandleIngredientDiscovered;
        CookingManager.Instance.CookingFinished += HandleCookingFinished;
        RecipeShopEvents.OnRecipeScrollUsed += HandleRecipeScrollUsed;
    }

    private void HandleRecipeScrollUsed(int recipeID)
    {
        // 첫 번째 시도: ResultID로 레시피 찾기 (현재 로직)
        var recipe = RecipeManager.Instance.RecipeList.Find(r => r.ResultID == recipeID);

        // 두 번째 시도: 레시피 ID 자체로 찾기 (백업 로직)
        if (recipe == null)
        {
            recipe = RecipeManager.Instance.RecipeList.Find(r => r.ID == recipeID);
            Debug.Log($"[RoomRecipeState] ResultID로 못 찾아서 레시피 ID로 찾기 시도: {recipeID}");
        }

        if (recipe == null)
        {
            Debug.LogWarning($"[RoomRecipeState] 레시피를 찾을 수 없습니다: recipeID={recipeID}");
            return;
        }

        Debug.Log($"[RoomRecipeState] 레시피 스크롤 사용 - 찾은 레시피: ID={recipe.ID}, ResultID={recipe.ResultID}");

        if (TryUnlockRecipe(recipe.ID))
        {
            OnRecipeUnlocked?.Invoke(recipe);

            // RecipeShopManager에 해금 알림 (구매 리스트에서 제거용)
            if (RecipeShopManager.Instance != null)
            {
                RecipeShopManager.Instance.OnRecipeUnlocked(recipe.ResultID);
            }
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

    public async void TryUnlockIngredient(int ingredientID)
    {
        if (IsUnlockedIngredients(ingredientID))
        {
            return;
        }

        var success = RoomInfoManager.Instance.CurrentRoomInfo.AddIngredient(ingredientID);
        if (success && HasStateAuthority)
        {
            try
            {
                await RoomInfoManager.Instance.Save();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            RPC_NotifyIngredientUnlocked(ingredientID);
        }

        return;
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyIngredientUnlocked(int ingredientID)
    {
        OnIngredientUnlocked?.Invoke(ingredientID);
    }

    public bool TryUnlockRecipe(int recipeID)
    {
        bool success = RoomInfoManager.Instance.CurrentRoomInfo.AddRecipe(recipeID);
        if (success && HasStateAuthority)
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
        TryUnlockIngredient(ingredientID);
    }
}
