using NUnit.Framework.Constraints;
using UnityEngine;

public class EnemyAttackState : IEnemyState<EnemyStateMachine_Deprecated>
{
    public bool IsInterruptable { get; } = true;

    private float _elapsedTime;
    private float _attackTime = 0.3f;
    
    public void Enter(EnemyStateMachine_Deprecated stateMachineDeprecated)
    {
        _elapsedTime = 0f;
    }

    public void Update(EnemyStateMachine_Deprecated stateMachineDeprecated, float deltaTime)
    {
        _elapsedTime += deltaTime;

        if (_elapsedTime > _attackTime)
        {
            stateMachineDeprecated.Animator.Play("Pounce Bite Attack W Root");
            stateMachineDeprecated.RequestStateChange(EEnemyState.Trace);
        }
    }

    public void Exit(EnemyStateMachine_Deprecated stateMachineDeprecated)
    {
        Debug.Log("ExitAttackState");
    }
}
