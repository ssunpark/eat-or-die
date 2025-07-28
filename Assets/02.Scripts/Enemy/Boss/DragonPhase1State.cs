using UnityEngine;

public class DragonPhase1State : IEnemyState<DragonStateMachine>
{
    public bool IsInterruptable => false;

    private IEnemyState<DragonStateMachine> _currentSubState;
    private DragonStateMachine _stateMachine;

    public void Enter(DragonStateMachine stateMachine)
    {
        Debug.Log("Phase1 진입: 드래곤이 공격을 시작합니다.");
        _stateMachine = stateMachine;
        SetSubState(ChooseRandomSubState());
    }

    public void Update(DragonStateMachine stateMachine, float deltaTime)
    {
        _currentSubState?.Update(stateMachine, deltaTime);
    }

    public void Exit(DragonStateMachine stateMachine)
    {
        _currentSubState?.Exit(stateMachine);
        Debug.Log("Phase1 종료: 드래곤이 다음 단계로 전환됩니다.");
    }

    public void OnSubStateComplete()
    {
        SetSubState(ChooseRandomSubState());
    }

    public void SetSubState(IEnemyState<DragonStateMachine> newState)
    {
        _currentSubState?.Exit(_stateMachine);
        _currentSubState = newState;
        _currentSubState.Enter(_stateMachine);
    }

    private IEnemyState<DragonStateMachine> ChooseRandomSubState()
    {
        int rand = Random.Range(0, 3);
        return rand switch
        {
            // 0 => new DragonFireAttackState(this),
            // 1 => new DragonTailAttackState(this),
            // 2 => new DragonCombatPatrolState(this),
            // _ => new DragonFireAttackState(this)
        };
    }
}