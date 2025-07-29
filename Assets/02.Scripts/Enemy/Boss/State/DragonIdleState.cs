using UnityEngine;

public class DragonIdleState : IEnemyState<DragonStateMachine>, IParentStateMachine
{
    private IEnemyState<DragonStateMachine> _currentSubState;
    private DragonStateMachine _stateMachine;

    public bool IsInterruptable => true;

    public void Enter(DragonStateMachine stateMachine)
    {
        Debug.Log("Idle 상태 진입");
        
        stateMachine.FightMode(false);
        
        _stateMachine = stateMachine;

        if (!stateMachine.IsLocked)
        {
            SetSubState(ChooseRandomIdleSubState());
        }
        else
        {
            _stateMachine.OnUnlock += OnUnlock;
        }
    }

    private void OnUnlock()
    {
        SetSubState(ChooseRandomIdleSubState());
        _stateMachine.OnUnlock -= OnUnlock;
    }

    public void Update(DragonStateMachine stateMachine, float deltaTime)
    {
        if (stateMachine.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        _currentSubState?.Update(stateMachine, deltaTime);
        if (stateMachine.Target != null)
        {
            stateMachine.ChangeState(EBossState.Alert);
        }
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
            0 => new DragonWaitState(this),
            1 => new DragonPatrolState(this),
            _ => new DragonWaitState(this)
        };
    }
}