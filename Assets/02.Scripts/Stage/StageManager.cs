using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StageManager : BehaviourSingleton<StageManager>
{
    [SerializeField] private List<Stage> _stages = new List<Stage>();
    private int _currentStage;
    public int CurrentStage => _currentStage;
    public event Action<string> OnStageAlert;

    public void Transfer(int from, int to)
    {
        ExitStage(from);
        EnterStage(to);
    }

    public void AlertCurrentStage()
    {
        OnStageAlert?.Invoke(_stages[_currentStage].StageName);
    }
    
    public void EnterStage(int stageIndex)
    {
        _currentStage = stageIndex;
        
        AlertCurrentStage();
        _stages[stageIndex]?.RPC_StageEnter(Room.Instance.Runner.LocalPlayer);
    }
    
    public void ExitStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= _stages.Count) return;
        _stages[stageIndex]?.RPC_StageExit(Room.Instance.Runner.LocalPlayer);
    }
}