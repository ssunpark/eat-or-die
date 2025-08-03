using UnityEngine;

public class EnemyIdleState : IEnemyState<EnemyStateMachine_Deprecated>
{
    public bool IsInterruptable => true;

    public void Enter(EnemyStateMachine_Deprecated stateMachineDeprecated)
    {
        Debug.Log("Entering Idle state");
        stateMachineDeprecated.Animator.Play("Idle");
    }

    public void Update(EnemyStateMachine_Deprecated stateMachineDeprecated, float deltaTime)
    {
        // OverlapSphere to check for player presence
        Collider[] hitColliders = Physics.OverlapSphere(stateMachineDeprecated.transform.position, 5f, LayerMask.GetMask("Player"));
        
        if (hitColliders.Length == 0) return;
        
        stateMachineDeprecated.SetTarget(hitColliders[0].gameObject);
        stateMachineDeprecated.RequestStateChange(EEnemyState.Trace);
    }

    public void Exit(EnemyStateMachine_Deprecated stateMachineDeprecated)
    {
        Debug.Log("Exiting Idle state");
    }
}