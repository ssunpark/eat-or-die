using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// 수현
public class RoomStateManager : NetworkBehaviourSingleton<RoomStateManager>
{
    private List<IRoomState> _roomStates = new List<IRoomState>();
    
    public void Register(IRoomState state)
    {
        if (!_roomStates.Contains(state))
        {
            _roomStates.Add(state);
            state.OnRegister();
        }
    }

    public void Unregister(IRoomState state)
    {
        _roomStates.Remove(state);
        state.OnUnregister();
    }

    public void CallAllPlayerJoined(PlayerRef player)
    {
        for (int i = 0; i < _roomStates.Count; i++)
        {
            _roomStates[i].OnPlayerJoined(player);
        }
    }

    public void CallAllPlayerLeft(PlayerRef player)
    {
        for (int i = 0; i < _roomStates.Count; i++)
        {
            _roomStates[i].OnPlayerLeft(player);
        }
    }
}
