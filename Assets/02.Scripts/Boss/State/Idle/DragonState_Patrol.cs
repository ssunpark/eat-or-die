using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Patrol : DragonSubStateBase
{
    private Vector3 _destination;
    private bool _hasDestination;

    private DragonStateParameterSet.PatrolParams _patrolParams;

    public DragonState_Patrol(DragonContext context, IParentState parent) : base(context, parent)
    {
        _patrolParams = Context.Parameter.Patrol;
    }

    protected override void OnEnterState()
    {
        _hasDestination = false;
    }

    protected override void OnFixedUpdate()
    {
        if (!_hasDestination || Context.Movement.Arrived())
        {
            SetNewDestination();
        }

        if (_hasDestination)
        {
            Context.Movement.Move(Machine.Runner.DeltaTime);
        }

        if (Machine.StateTime >= _patrolParams.PatrolDuration)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnExitState()
    {
        Context.Movement.ResetNavMeshAgent();
    }

    protected override void OnEnterStateRender()
    {
        Context.Animator.SetBool("IsMove", true);
    }

    protected override void OnExitStateRender()
    {
        Context.Animator.SetBool("IsMove", false);
    }

    private void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized * Random.Range(_patrolParams.MinWalkRadius, _patrolParams.WalkRadius);
        randomDirection += Context.Transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _patrolParams.WalkRadius, NavMesh.AllAreas))
        {
            _destination = hit.position;
            Context.Movement.SetDestination(_destination);
            _hasDestination = true;
        }
    }
}