using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Alert : DragonStateBase
{
    private DragonStateParameterSet.AlertParams _alertParams;
    private DragonStateParameterSet.BaseParams _baseParams;

    private bool _hasDestination;

    public DragonState_Alert(DragonContext context)
        : base(context)
    {
        _alertParams = Context.Parameter.Alert;
        _baseParams = Context.Parameter.Base;
    }

    protected override void OnEnterState()
    {
        Context.Movement.SetNavMeshAgentMoveData(_baseParams.MoveSpeed, _baseParams.RotationSpeed);

        _hasDestination = false;

        Context.Animator.SetBool("IsMove", true);
    }

    protected override void OnFixedUpdate()
    {
        if (!_hasDestination || Context.Movement.Arrived())
        {
            ChooseNewLookDestination();
        }

        Context.Movement.Move(Machine.Runner.DeltaTime);

        if (Machine.StateTime >= _alertParams.LookDuration)
        {
            HandleAlertDecision();
        }
    }

    private void HandleAlertDecision()
    {
        // Machine.TryActivateState<DragonState_MagicAttack>(true);
        // return;
        float distance = Context.Sight.Distance;
        float rand = Random.value;
        
        // 너무 멀면 Chase or 원거리 마법 시도
        if (distance > _baseParams.MeleeAttackDistance)
        {
            if (rand < _alertParams.ChaseProbability)
            {
                Machine.TryActivateState<DragonState_Chase>(true);
                return;
            }
            else if (rand < _alertParams.ChaseProbability + _alertParams.MagicProbability)
            {
                Machine.TryActivateState<DragonState_MagicAttack>(true);
                return;
            }
        }

        // 아닌 경우 확률에 따라 마법 or 근접 공격
        rand = Random.value;
        if (rand < _alertParams.MagicProbability)
        {
            Machine.TryActivateState<DragonState_MagicAttack>(true);
        }
        else
        {
            Machine.TryActivateState<DragonState_MeleeAttack>(true);
        }
    }

    protected override void OnExitState()
    {
        Context.Animator.SetBool("IsMove", false);
        Context.Movement.ResetNavMeshAgent();
    }

    private void ChooseNewLookDestination()
    {
        Vector3 center = Context.Sight.Target.transform.position;
        Vector3 dir = (Context.Sight.Target.transform.position - center).normalized;

        int randomSign = Random.value < 0.5f ? -1 : 1;
        float offsetAngle = randomSign * Random.Range(_alertParams.MinAngleRange, _alertParams.AngleRange);
        Vector3 rotatedDir = Quaternion.Euler(0f, offsetAngle, 0f) * dir;

        float distance = Context.Sight.Distance + Random.Range(-_alertParams.WalkRange, _alertParams.WalkRange);

        distance = Mathf.Max(_alertParams.MinDistance, distance);

        Vector3 destination = center + rotatedDir * distance;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Context.Movement.SetDestination(hit.position);
            _hasDestination = true;
        }
    }
}