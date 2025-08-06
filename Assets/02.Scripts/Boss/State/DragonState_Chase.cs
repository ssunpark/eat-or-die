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

    public DragonState_Chase(DragonController controller, DragonParameterLoader paramLoader)
        : base(controller, paramLoader)
    {
        _chaseParams = paramLoader.Chase;
        _baseParams = paramLoader.Base;
    }

    protected override void OnEnterState()
    {
        Controller.SetNavMeshAgentMoveData(_chaseParams.ChaseSpeed, _chaseParams.RotationSpeed);
        Controller.Animator.SetBool("IsMove", true);

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
        if (Controller.IsLocked || Controller.Target == null) return;

        float distance = Vector3.Distance(Controller.transform.position, Controller.Target.transform.position);

        if (distance <= _baseParams.MeleeAttackDistance)
        {
            Machine.TryActivateState<DragonState_MeleeAttack>(true);
            return;
        }

        if (Controller.SightDetector.DetectedColliders.Count == 0)
        {
            Machine.TryActivateState<DragonState_Alert>(true);
            return;
        }

        if (!_sidestepComplete)
        {
            if (Arrived())
            {
                _sidestepComplete = true;
            }
            else
            {
                Controller.SetDestination(_sidestepPosition);
            }
        }

        if (_sidestepComplete)
        {
            Controller.SetDestination(Controller.Target.transform.position);
        }

        Controller.Move(Machine.Runner.DeltaTime);
    }

    private void SetSidestepDestination()
    {
        Vector3 center = Controller.Target.transform.position;
        Vector3 dir = (Controller.transform.position - center).normalized;

        int sign = Random.value < 0.5f ? -1 : 1;
        float angle = sign * Random.Range(_chaseParams.MinSidestepAngle, _chaseParams.MaxSidestepAngle);
        Vector3 rotatedDir = Quaternion.Euler(0f, angle, 0f) * dir;

        float dist = Vector3.Distance(center, Controller.transform.position)
                     + Random.Range(-_chaseParams.SidestepRange, _chaseParams.SidestepRange);
        dist = Mathf.Max(_chaseParams.MinSidestepDistance, dist);

        Vector3 destination = center + rotatedDir * dist;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            _sidestepPosition = hit.position;
            Controller.SetDestination(_sidestepPosition);
        }
        else
        {
            // 실패 시 바로 추적
            _sidestepComplete = true;
        }
    }

    private bool Arrived()
    {
        return !Controller.NavMeshAgent.pathPending &&
               Controller.NavMeshAgent.remainingDistance <= Controller.NavMeshAgent.stoppingDistance;
    }

    protected override void OnExitState()
    {
        Controller.SetNavMeshAgentMoveData(_baseParams.MoveSpeed, _baseParams.RotationSpeed);
        
        Controller.ResetNavMeshAgent();
        Controller.Animator.SetBool("IsMove", false);
    }
}
