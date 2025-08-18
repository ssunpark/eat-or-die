using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StageManager : BehaviourSingleton<StageManager>
{
    [SerializeField] private List<Stage> _stages = new List<Stage>();

    public void Transfer(int from, int to)
    {
        ExitStage(from);
        EnterStage(to);
    }
    
    public void EnterStage(int stageIndex)
    {
        _stages[stageIndex]?.RPC_StageEnter(Room.Instance.Runner.LocalPlayer);
    }
    
    public void ExitStage(int stageIndex)
    {
        _stages[stageIndex]?.RPC_StageExit(Room.Instance.Runner.LocalPlayer);
    }
}