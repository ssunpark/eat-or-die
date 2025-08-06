using System.Collections.Generic;
using Fusion;
using UnityEngine;
// 수현
public class RoomRecipeState : IRoomState
{
    private HashSet<int> _unlockedRecipes = new HashSet<int>();
    
    public void OnRegister()
    {
        _unlockedRecipes.Clear();
    }

    public void OnUnregister()
    {
        _unlockedRecipes.Clear();
    }

    public void UnlockRecipe(int recipeID)
    {
        _unlockedRecipes.Add(recipeID);
    }

    public bool IsUnlocked(int recipeID)
    {
        return _unlockedRecipes.Contains(recipeID);
    }
    
    public void OnPlayerJoined(PlayerRef player)
    {
        // 이 예제에서는 네트워크 전송 생략, 필요 시 RPC나 이벤트로 처리
        Debug.Log($"[RoomRecipeState] Send unlocked recipes to {player}");
    }

    public void OnPlayerLeft(PlayerRef player)
    {
        throw new System.NotImplementedException();
    }
    
    
}
