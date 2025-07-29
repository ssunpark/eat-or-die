using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Patrol : DragonSubStateBase
{
    private float _patrolDuration = 5f;
    private float _walkRadius = 10f;

    private Vector3 _destination;
    private bool _hasDestination;

    public DragonState_Patrol(DragonController controller, IParentState parent) : base(controller, parent)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("Idle 배회 상태 진입");
        _hasDestination = false;

        Controller.Animator.SetBool("IsMove", true);
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        if (!_hasDestination || Arrived())
        {
            SetNewDestination();
        }

        if (_hasDestination && !Controller.NavMeshAgent.pathPending)
        {
            Controller.Move(Machine.Runner.DeltaTime);
        }

        if (Machine.StateTime >= _patrolDuration)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnExitState()
    {
        Debug.Log("Idle 배회 상태 종료");
        Controller.NavMeshAgent.ResetPath();
        Controller.NavMeshAgent.velocity = Vector3.zero;
        
        Controller.Animator.SetBool("IsMove", false);
    }

    private bool Arrived()
    {
        return !Controller.NavMeshAgent.pathPending &&
               Controller.NavMeshAgent.remainingDistance <= Controller.NavMeshAgent.stoppingDistance;
    }

    private void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized * Random.Range(5f, _walkRadius);
        randomDirection += Controller.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _walkRadius, NavMesh.AllAreas))
        {
            _destination = hit.position;
            Controller.NavMeshAgent.SetDestination(_destination);
            _hasDestination = true;
        }
    }
}