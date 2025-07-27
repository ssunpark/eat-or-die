using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
        stateMachine.Animator.Play("Run Forward In Place");
    }

    public void Update(EnemyStateMachine stateMachine, float deltaTime)
    {
        if (stateMachine.Target == null)
        {
            stateMachine.RequestStateChange(EEnemyState.Idle);
        }
        
        float distance = Vector3.Distance(stateMachine.Target.transform.position, stateMachine.transform.position);
        if (distance < stateMachine.AttackRange)
        {
            stateMachine.RequestStateChange(EEnemyState.Attack);
        }
        
        Vector3 direction = stateMachine.NavMeshAgent.nextPosition - stateMachine.transform.position;
        stateMachine.Move(direction);
        stateMachine.NavMeshAgent.SetDestination(stateMachine.Target.transform.position);
    }

    public void Exit(EnemyStateMachine stateMachine)
    {
        stateMachine.Animator.Play("Idle");
    }
}
        