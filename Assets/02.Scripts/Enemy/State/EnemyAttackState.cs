using NUnit.Framework.Constraints;
using UnityEngine;

public class EnemyAttackState : IEnemyState<EnemyStateMachine>
{
    public bool IsInterruptable { get; } = true;

    private float _elapsedTime;
    private float _attackTime = 0.3f;
    
    public void Enter(EnemyStateMachine stateMachine)
    {
        _elapsedTime = 0f;
    }

    public void Update(EnemyStateMachine stateMachine, float deltaTime)
    {
        _elapsedTime += deltaTime;

        if (_elapsedTime > _attackTime)
        {
            stateMachine.Animator.Play("Pounce Bite Attack W Root");
            stateMachine.RequestStateChange(EEnemyState.Trace);
        }
    }

    public void Exit(EnemyStateMachine stateMachine)
    {
        Debug.Log("ExitAttackState");
    }
}
