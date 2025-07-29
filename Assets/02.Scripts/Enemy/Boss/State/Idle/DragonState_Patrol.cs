using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Patrol : DragonSubStateBase
{
    private float _patrolDuration = 5f;
    private float _walkRadius = 10f;

    private Vector3 _destination;
    private bool _hasDestination;

    public DragonState_Patrol(DragonStateMachine machine, IParentStateMachine parentMachine) : base(machine, parentMachine)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("Idle 배회 상태 진입");
        _hasDestination = false;

        StateMachine.Animator.SetBool("IsMove", true);
    }

    protected override void OnFixedUpdate()
    {
        if (StateMachine.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        if (!_hasDestination || Arrived())
        {
            SetNewDestination();
        }

        if (_hasDestination && !StateMachine.NavMeshAgent.pathPending)
        {
            StateMachine.Move(Machine.Runner.DeltaTime);
        }

        if (Machine.StateTime >= _patrolDuration)
        {
            ParentStateMachine.OnSubStateComplete();
        }
    }

    protected override void OnExitState()
    {
        Debug.Log("Idle 배회 상태 종료");
        StateMachine.NavMeshAgent.ResetPath();
        StateMachine.NavMeshAgent.velocity = Vector3.zero;
        
        StateMachine.Animator.SetBool("IsMove", false);
    }

    private bool Arrived()
    {
        return !StateMachine.NavMeshAgent.pathPending &&
               StateMachine.NavMeshAgent.remainingDistance <= StateMachine.NavMeshAgent.stoppingDistance;
    }

    private void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized * Random.Range(5f, _walkRadius);
        randomDirection += StateMachine.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _walkRadius, NavMesh.AllAreas))
        {
            _destination = hit.position;
            StateMachine.NavMeshAgent.SetDestination(_destination);
            _hasDestination = true;
        }
    }
}