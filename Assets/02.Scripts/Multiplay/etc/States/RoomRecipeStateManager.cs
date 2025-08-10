using System;
using System.Linq;

public class RoomRecipeStateManager : BehaviourSingleton<RoomRecipeStateManager>
{
    public static event Action<Recipe> OnRecipeUnlocked;
    public static event Action<int> OnIngredientUnlocked;

    private void OnEnable()
    {
        InventoryManager.OnItemAcquired += HandleIngredientDiscovered;
        CookingManager.CookingFinished += HandleCookingFinished;

    }
    
    private void OnDisable()
    {
        InventoryManager.OnItemAcquired -= HandleIngredientDiscovered;
        CookingManager.CookingFinished -= HandleCookingFinished;
    
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

            // 저장이 성공했을 때, 이 메서드가 직접 이벤트를 발생시킵니다.
            OnIngredientUnlocked?.Invoke(ingredientID);
        }

        return success;
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
        TryUnlockIngredient(acquiredItem.ID);
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
}
