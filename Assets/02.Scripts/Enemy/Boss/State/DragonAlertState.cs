using UnityEditor.Animations;
using UnityEngine;

public class DragonAlertState : IEnemyState<DragonStateMachine>
{
    private const string ANIMATION_LAYER_FIGHT = "Fight Layer";
    
    private IEnemyState<DragonStateMachine> _currentSubState;
    private DragonStateMachine _stateMachine;

    public bool IsInterruptable => true;

    public void Enter(DragonStateMachine stateMachine)
    {
        Debug.Log("Alert 상태 진입");

        stateMachine.FightMode(true);

        _stateMachine = stateMachine;
        SetSubState(new DragonLookState(this));
    }

    public void Update(DragonStateMachine stateMachine, float deltaTime)
    {
        if (stateMachine.Target == null)
        {
            stateMachine.ChangeState(EBossState.Idle);
        }
        _currentSubState?.Update(stateMachine, deltaTime);
    }

    public void Exit(DragonStateMachine stateMachine)
    {
        _currentSubState?.Exit(stateMachine);
    }

    public void OnSubStateComplete()
    {
        SetSubState(new DragonLookState(this));
    }

    private void SetSubState(IEnemyState<DragonStateMachine> newSubState)
    {
        _currentSubState?.Exit(_stateMachine);
        _currentSubState = newSubState;
        _currentSubState?.Enter(_stateMachine);
    }

    //
    // private IEnemyState<DragonStateMachine> ChooseNextAlertSubState()
    // {
    //     // 공격 전 Ready 또는 상황에 따라 Chase 등 선택
    //     return new DragonReadyState(this);
    // }
}