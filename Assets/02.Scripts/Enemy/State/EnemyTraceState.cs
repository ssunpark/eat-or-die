using UnityEngine;

public class EnemyTraceState : IEnemyState<EnemyStateMachine>
{
    public bool IsInterruptable { get; } = true;
    
    public void Enter(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Trace state");
        if (stateMachine.Target == null)
        {
            stateMachine.RequestStateChange(EEnemyState.Idle);
        }
    }

    public void Update(EnemyStateMachine stateMachine, float deltaTime)
    {
        if (stateMachine.Target == null)
        {
            stateMachine.RequestStateChange(EEnemyState.Idle);
        }

        // stateMachine.Rigidbody.position = stateMachine.NavMeshAgent.nextPosition;
        stateMachine.NavMeshAgent.SetDestination(stateMachine.Target.transform.position);
    }

    public void Exit(EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Trace state");
    }
}
        