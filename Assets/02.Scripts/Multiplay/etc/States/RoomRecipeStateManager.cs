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
    }

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
        if (IsUnlockedIngredients(ingredientID))
        {
            return;
        }

        var success = RoomInfoManager.Instance.CurrentRoomInfo.AddIngredient(ingredientID);
        if (success && HasStateAuthority)
        {
            await RoomInfoManager.Instance.Save();
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
