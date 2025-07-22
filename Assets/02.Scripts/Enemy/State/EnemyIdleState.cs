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
        // OverlapSphere to check for player presence
        Collider[] hitColliders = Physics.OverlapSphere(stateMachine.transform.position, 5f, LayerMask.GetMask("Player"));
        
        if (hitColliders.Length == 0) return;
        
        stateMachine.SetTarget(hitColliders[0].gameObject);
        stateMachine.RequestStateChange(EEnemyState.Trace);
    }

    public void Exit(EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Idle state");
    }
}