using UnityEngine;
using UnityEngine.AI;

public class DragonPatrolState : IEnemyState<DragonStateMachine>
{
    private readonly IParentStateMachine _parent;
    private float _timer = 5f;
    private float _walkRadius = 10f;

    private Vector3 _destination;
    private bool _hasDestination;

    public bool IsInterruptable => true;

    public DragonPatrolState(IParentStateMachine parent)
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
            stateMachine.Move(dt);
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
        stateMachine.NavMeshAgent.velocity = Vector3.zero;
        
        stateMachine.Animator.SetBool("IsMove", false);
    }

    private bool Arrived(DragonStateMachine stateMachine)
    {
        return !stateMachine.NavMeshAgent.pathPending &&
               stateMachine.NavMeshAgent.remainingDistance <= stateMachine.NavMeshAgent.stoppingDistance;
    }

    private void SetNewDestination(DragonStateMachine stateMachine)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized * Random.Range(5f, _walkRadius);
        randomDirection += stateMachine.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _walkRadius, NavMesh.AllAreas))
        {
            _destination = hit.position;
            stateMachine.NavMeshAgent.SetDestination(_destination);
            _hasDestination = true;
        }
    }
}