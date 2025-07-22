using UnityEngine;

public class EnemyIdleState : IEnemyState<EnemyStateMachine>
{
    public bool IsInterruptable => true;

    public void Enter(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Idle state");
    }

    public void Update(EnemyStateMachine stateMachine, float deltaTime)
    {
    }

    public void Exit(EnemyStateMachine stateMachine)
    {
    }
}