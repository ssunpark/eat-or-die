using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Patrol : DragonSubStateBase
{
    private Vector3 _destination;
    private bool _hasDestination;

    private DragonStateParameterSet.PatrolParams _patrolParams;

    public DragonState_Patrol(DragonController controller, IParentState parent, DragonStateParameterSet.PatrolParams patrolParams) : base(controller, parent)
    {
        _patrolParams = patrolParams;
    }

    protected override void OnEnterState()
    {
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

        if (Machine.StateTime >= _patrolParams.PatrolDuration)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnExitState()
    {
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
        Vector3 randomDirection = Random.insideUnitSphere.normalized * Random.Range(_patrolParams.MinWalkRadius, _patrolParams.WalkRadius);
        randomDirection += Controller.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _patrolParams.WalkRadius, NavMesh.AllAreas))
        {
            _destination = hit.position;
            Controller.SetDestination(_destination);
            _hasDestination = true;
        }
    }
}