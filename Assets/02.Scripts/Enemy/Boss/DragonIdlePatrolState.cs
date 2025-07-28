using UnityEngine;
using UnityEngine.AI;

public class DragonIdlePatrolState : IEnemyState<DragonStateMachine>
{
    private readonly DragonIdleState _parent;
    private float _timer = 5f;
    private float _walkRadius = 10f;

    private Vector3 _destination;
    private bool _hasDestination;

    public bool IsInterruptable => true;

    public DragonIdlePatrolState(DragonIdleState parent)
    {
        _parent = parent;
    }

    public void Enter(DragonStateMachine stateMachine)
    {
        Debug.Log("Idle 배회 상태 진입");
        _hasDestination = false;

        stateMachine.Animator.SetBool("IsMove", true);
    }

    public void Update(DragonStateMachine stateMachine, float dt)
    {
        _timer -= dt;

        if (!_hasDestination || Arrived(stateMachine))
        {
            SetNewDestination(stateMachine);
        }

        if (_hasDestination && !stateMachine.NavMeshAgent.pathPending)
        {
            Debug.DrawLine(stateMachine.NavMeshAgent.nextPosition, stateMachine.NavMeshAgent.nextPosition + Vector3.up * 3f, Color.red, 5f);
            Vector3 direction = stateMachine.NavMeshAgent.nextPosition - stateMachine.transform.position;
            stateMachine.Move(direction);
        }

        if (_timer <= 0f)
        {
            _parent.OnSubStateComplete();
        }
    }

    public void Exit(DragonStateMachine stateMachine)
    {
        Debug.Log("Idle 배회 상태 종료");
        stateMachine.NavMeshAgent.ResetPath();
        
        stateMachine.Animator.SetBool("IsMove", false);
    }

    private bool Arrived(DragonStateMachine stateMachine)
    {
        return !stateMachine.NavMeshAgent.pathPending &&
               stateMachine.NavMeshAgent.remainingDistance <= stateMachine.NavMeshAgent.stoppingDistance;
    }

    private void SetNewDestination(DragonStateMachine stateMachine)
    {
        Vector3 randomDirection = Random.insideUnitSphere * _walkRadius + Vector3.one * 5f;
        randomDirection += stateMachine.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _walkRadius, NavMesh.AllAreas))
        {
            _destination = hit.position;
            stateMachine.NavMeshAgent.SetDestination(_destination);
            _hasDestination = true;
        }
    }
}