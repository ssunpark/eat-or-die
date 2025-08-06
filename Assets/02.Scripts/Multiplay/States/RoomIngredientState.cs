using System.Collections.Generic;
using Fusion;
using UnityEngine;
// 수현
public class RoomIngredientState : IRoomState
{
    private HashSet<int> _knownIngredients = new HashSet<int>();

    public RoomIngredientState()
    {
        RoomStateManager.Instance.Register(this);
    }
    
    public void OnRegister()
    {
        _knownIngredients.Clear();
    }

    public void OnUnregister()
    {
        _knownIngredients.Clear();
    }


    public void RegisterIngredient(int ingredientID)
    {
        _knownIngredients.Add(ingredientID);
    }

    public bool IsKnown(int ingredientID)
    {
        return _knownIngredients.Contains(ingredientID);
    }
    
    public void OnPlayerJoined(PlayerRef player)
    {
        // 습득한 재료 클라이언트에 동기화
        Debug.Log($"[RoomIngredientState] Send known ingredients to {player}");
    }

    public void OnPlayerLeft(PlayerRef player)
    {
        throw new System.NotImplementedException();
    }
}
