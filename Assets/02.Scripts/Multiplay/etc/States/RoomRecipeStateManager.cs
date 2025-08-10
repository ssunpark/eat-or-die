using System.Linq;
using UnityEngine;

public class RoomRecipeStateManager : BehaviourSingleton<RoomRecipeStateManager>
{
    public static event System.Action<Recipe> OnRecipeUnlocked;
    
    private void OnEnable()
    {
        CookingManager.CookingFinished += HandleCookingFinished;

    }
    
    private void OnDisable()
    {
        CookingManager.CookingFinished -= HandleCookingFinished;
    
    }
    
    public bool IsUnlocked(int recipeID)
    {
        return RoomInfoManager.Instance.CurrentRoomInfo.KnownRecipes.Contains(recipeID);
    }

    public bool TryUnlock(int recipeID)
    {
        // if (IsUnlocked(recipeID)) return false;

        bool success = RoomInfoManager.Instance.CurrentRoomInfo.AddRecipe(recipeID);
        if (success)
        {
            RoomInfoManager.Instance.Save();
        }
        return success;
    }
    
    private void HandleCookingFinished(ItemInstance cookedItem)
    {
        Debug.Log("HandleCookingFinished 메서드 호출!!");
        var recipe = RecipeManager.Instance.RecipeList.Find(r => r.ResultID == cookedItem.ID);
        if (recipe == null) return;
    
        if (TryUnlock(recipe.ID))
        {
            Debug.Log("룸레시피메니저에서 TryUnlock 시도하고 리프레시!!");
            OnRecipeUnlocked?.Invoke(recipe);
        }
    }
}
