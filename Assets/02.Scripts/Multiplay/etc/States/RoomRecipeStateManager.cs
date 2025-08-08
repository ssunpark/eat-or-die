using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomRecipeStateManager : BehaviourSingleton<RoomRecipeStateManager>
{
    private List<UI_RecipeButton> _recipeButtonList = new List<UI_RecipeButton>();

    private void OnEnable()
    {
        CookingManager.CookingFinished += HandleCookingFinished;
        InventoryManager.Instance.OnInventoryUpdated += RefreshAllButtons;

    }
    
    private void OnDisable()
    {
        // CookingManager.CookingFinished -= HandleCookingFinished;
        // InventoryManager.Instance.OnInventoryUpdated -= RefreshAllButtons;

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
    
    public void RefreshAllButtons()
    {
        Debug.Log("RefreshAllButtons");
        foreach (var button in _recipeButtonList)
        {
            Debug.Log("버튼 리프레시 !");
            button.Refresh(button.GetRecipe());
        }
    }
    
    private void HandleCookingFinished(Item cookedItem)
    {
        Debug.Log("HandleCookingFinished 메서드 호출!!");
        var recipe = RecipeManager.Instance.RecipeList.Find(r => r.ResultID == cookedItem.ID);
        if (recipe == null) return;
    
        if (TryUnlock(recipe.ID))
        {
            Debug.Log("룸레시피메니저에서 TryUnlock 시도하고 리프레시!!");
            RefreshAllButtons();
        }
    }
}
