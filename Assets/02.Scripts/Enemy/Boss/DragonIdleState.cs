using UnityEngine;

public class DragonIdleState : IEnemyState<DragonStateMachine>
{
    private IEnemyState<DragonStateMachine> _currentSubState;
    private DragonStateMachine _stateMachine;

    public bool IsInterruptable => true;

    public void Enter(DragonStateMachine stateMachine)
    {
        Debug.Log("Idle 상태 진입");

        _stateMachine = stateMachine;
        SetSubState(ChooseRandomIdleSubState());
    }

    public void Update(DragonStateMachine stateMachine, float deltaTime)
    {
        _currentSubState?.Update(stateMachine, deltaTime);
    }

    public void Exit(DragonStateMachine stateMachine)
    {
        _currentSubState?.Exit(stateMachine);
    }
    
    public void OnSubStateComplete()
    {
        SetSubState(ChooseRandomIdleSubState());
    }

    private void SetSubState(IEnemyState<DragonStateMachine> newSubState)
    {
        _currentSubState?.Exit(_stateMachine);
        _currentSubState = newSubState;
        _currentSubState?.Enter(_stateMachine);
    }

    private IEnemyState<DragonStateMachine> ChooseRandomIdleSubState()
    {
        int rand = Random.Range(0, 2);
        return rand switch
        {
            0 => new DragonIdleWaitState(this),
            1 => new DragonIdlePatrolState(this),
            _ => new DragonIdleWaitState(this)
        };
    }
}