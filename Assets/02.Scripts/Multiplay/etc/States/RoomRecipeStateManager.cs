using UnityEngine;

public class RoomRecipeStateManager : BehaviourSingleton<RoomRecipeStateManager>
{
    public bool IsUnlocked(int recipeID)
    {
        return RoomInfoManager.Instance.CurrentRoomInfo.KnownRecipes.Contains(recipeID);
    }

    public bool TryUnlock(int recipeID)
    {
        if (IsUnlocked(recipeID)) return false;

        RoomInfoManager.Instance.CurrentRoomInfo.KnownRecipes.Add(recipeID);
        RoomInfoManager.Instance.Save();
        return true;
    }
}
