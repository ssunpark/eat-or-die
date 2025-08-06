using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Chase : DragonStateBase
{
    private DragonStateParameterSet.ChaseParams _chaseParams;
    private DragonStateParameterSet.BaseParams _baseParams;

    private bool _sidestepComplete;
    private bool _doSidestep;
    private Vector3 _sidestepPosition;

    public DragonState_Chase(DragonContext context)
        : base(context)
    {
        _chaseParams = Context.Parameter.Chase;
        _baseParams = Context.Parameter.Base;
    }

    protected override bool CanEnterState()
    {
        return Context.Sight.HasTarget;
    }

    protected override void OnEnterState()
    {
        Context.Movement.SetNavMeshAgentMoveData(_chaseParams.ChaseSpeed, _chaseParams.RotationSpeed);
        Context.Animator.SetBool("IsMove", true);

        // 확률적으로 sidestep 시도 여부 결정
        _doSidestep = Random.value < _chaseParams.SidestepProbability;
        _sidestepComplete = !_doSidestep; // sidestep 안 할 거면 바로 완료 처리

        if (_doSidestep)
        {
            SetSidestepDestination();
        }
    }

    protected override void OnFixedUpdate()
    {
        float distance = Context.Sight.Distance;

        if (distance <= _baseParams.MeleeAttackDistance)
        {
            Machine.TryActivateState<DragonState_MeleeAttack>(true);
            return;
        }

        if (Context.Sight.SightDetector.DetectedColliders.Count == 0)
        {
            Machine.TryActivateState<DragonState_Alert>(true);
            return;
        }

        if (!_sidestepComplete)
        {
            if (Context.Movement.Arrived())
            {
                _sidestepComplete = true;
            }
            else
            {
                Context.Movement.SetDestination(_sidestepPosition);
            }
        }

        if (_sidestepComplete)
        {
            Context.Movement.SetDestination(Context.Sight.Target.transform.position);
        }

        Context.Movement.Move(Machine.Runner.DeltaTime);
    }

    private void SetSidestepDestination()
    {
        Vector3 center = Context.Sight.Target.transform.position;
        Vector3 dir = (Context.Sight.Target.transform.position - center).normalized;

        int sign = Random.value < 0.5f ? -1 : 1;
        float angle = sign * Random.Range(_chaseParams.MinSidestepAngle, _chaseParams.MaxSidestepAngle);
        Vector3 rotatedDir = Quaternion.Euler(0f, angle, 0f) * dir;

        float dist = Context.Sight.Distance + Random.Range(-_chaseParams.SidestepRange, _chaseParams.SidestepRange);
        dist = Mathf.Max(_chaseParams.MinSidestepDistance, dist);

        Vector3 destination = center + rotatedDir * dist;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            _sidestepPosition = hit.position;
            Context.Movement.SetDestination(_sidestepPosition);
        }
        else
        {
            // 실패 시 바로 추적
            _sidestepComplete = true;
        }
    }
    
    protected override void OnExitState()
    {
        Context.Movement.SetNavMeshAgentMoveData(_baseParams.MoveSpeed, _baseParams.RotationSpeed);

        Context.Movement.ResetNavMeshAgent();
        Context.Animator.SetBool("IsMove", false);
    }
}