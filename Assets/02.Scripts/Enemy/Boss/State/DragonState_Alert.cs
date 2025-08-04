using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Alert : DragonStateBase
{
    private DragonStateParameterSet.AlertParams _alertParams;
    private DragonStateParameterSet.BaseParams _baseParams;

    private bool _hasDestination;

    public DragonState_Alert(DragonController controller, DragonParameterLoader paramLoader)
        : base(controller, paramLoader)
    {
        _alertParams = paramLoader.Alert;
        _baseParams = paramLoader.Base;
    }

    protected override void OnEnterState()
    {
        Controller.SetNavMeshAgentMoveData(_baseParams.MoveSpeed, _baseParams.RotationSpeed);
        
        _hasDestination = false;
        
        Controller.Animator.SetBool("IsMove", true);
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked) return;

        if (!_hasDestination || Arrived())
        {
            ChooseNewLookDestination();
        }

        Controller.Move(Machine.Runner.DeltaTime);

        if (Machine.StateTime >= _alertParams.LookDuration)
        {
            HandleAlertDecision();
        }
    }

    private void HandleAlertDecision()
    {
        Machine.TryActivateState<DragonState_MagicAttack>(true);
        return;
        float distance = Vector3.Distance(Controller.transform.position, Controller.Target.transform.position);
        
        if (distance <= _baseParams.MeleeAttackDistance)
        {
            Machine.TryActivateState<DragonState_MeleeAttack>(true);
            return;
        }
        
        float rand = Random.value;
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
        Machine.TryActivateState<DragonState_Alert>(true);
    }

    protected override void OnExitState()
    {
        Controller.Animator.SetBool("IsMove", false);
        Controller.ResetNavMeshAgent();
    }

    private void ChooseNewLookDestination()
    {
        Vector3 center = Controller.Target.transform.position;
        Vector3 dir = (Controller.transform.position - center).normalized;

        int randomSign = Random.value < 0.5f ? -1 : 1;
        float offsetAngle = randomSign * Random.Range(_alertParams.MinAngleRange, _alertParams.AngleRange);
        Vector3 rotatedDir = Quaternion.Euler(0f, offsetAngle, 0f) * dir;

        float distance = Vector3.Distance(center, Controller.transform.position)
                         + Random.Range(-_alertParams.WalkRange, _alertParams.WalkRange);
        
        distance = Mathf.Max(_alertParams.MinDistance, distance);
        
        Vector3 destination = center + rotatedDir * distance;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Controller.SetDestination(hit.position);
            _hasDestination = true;
        }
    }

    private bool Arrived()
    {
        return !Controller.NavMeshAgent.pathPending &&
               Controller.NavMeshAgent.remainingDistance <= Controller.NavMeshAgent.stoppingDistance;
    }
}
