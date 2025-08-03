using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemyTraceState : IEnemyState<EnemyStateMachine_Deprecated>
{
    public bool IsInterruptable { get; } = true;
    
    public void Enter(EnemyStateMachine_Deprecated stateMachineDeprecated)
    {
        if (stateMachineDeprecated.Target == null)
        {
            stateMachineDeprecated.RequestStateChange(EEnemyState.Idle);
            return;
        }
        stateMachineDeprecated.NavMeshAgent.SetDestination(stateMachineDeprecated.Target.transform.position);
        stateMachineDeprecated.Animator.Play("Run Forward In Place");
    }

    public void Update(EnemyStateMachine_Deprecated stateMachineDeprecated, float deltaTime)
    {
        if (stateMachineDeprecated.Target == null)
        {
            stateMachineDeprecated.RequestStateChange(EEnemyState.Idle);
        }
        
        float distance = Vector3.Distance(stateMachineDeprecated.Target.transform.position, stateMachineDeprecated.transform.position);
        if (distance < stateMachineDeprecated.AttackRange)
        {
            stateMachineDeprecated.RequestStateChange(EEnemyState.Attack);
        }
        
        Vector3 direction = stateMachineDeprecated.NavMeshAgent.nextPosition - stateMachineDeprecated.transform.position;
        stateMachineDeprecated.Move(direction);
        stateMachineDeprecated.NavMeshAgent.SetDestination(stateMachineDeprecated.Target.transform.position);
    }

    public void Exit(EnemyStateMachine_Deprecated stateMachineDeprecated)
    {
    }
}
        