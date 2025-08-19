using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StageManager : BehaviourSingleton<StageManager>
{
    [SerializeField] private List<Stage> _stages = new List<Stage>();
    private int _currentStage;

    public void Transfer(int from, int to)
    {
        ExitStage(from);
        EnterStage(to);
    }

    public void AlertCurrentStage()
    {
        _stages[_currentStage].AlertStageName();
    }
    
    public void EnterStage(int stageIndex)
    {
        _currentStage = stageIndex;
        
        AlertCurrentStage();
        _stages[stageIndex]?.RPC_StageEnter(Room.Instance.Runner.LocalPlayer);
    }
    
    public void ExitStage(int stageIndex)
    {
        _stages[stageIndex]?.RPC_StageExit(Room.Instance.Runner.LocalPlayer);
    }
}