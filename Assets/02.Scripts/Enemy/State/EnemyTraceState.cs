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
            return;
        }
        stateMachine.NavMeshAgent.SetDestination(stateMachine.Target.transform.position);
    }

    public void Update(EnemyStateMachine stateMachine, float deltaTime)
    {
        if (stateMachine.Target == null)
        {
            stateMachine.RequestStateChange(EEnemyState.Idle);
        }
        
        Vector3 direction = stateMachine.NavMeshAgent.nextPosition - stateMachine.transform.position;
        stateMachine.Move(direction);
        stateMachine.NavMeshAgent.SetDestination(stateMachine.Target.transform.position);
    }

    public void Exit(EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Trace state");
    }
}
        