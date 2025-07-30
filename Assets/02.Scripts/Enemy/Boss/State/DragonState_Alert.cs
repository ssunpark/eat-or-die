using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Alert : DragonStateBase
{
    private DragonStateParameterSet.AlertParams _alertParams;

    private bool _hasDestination;

    public DragonState_Alert(DragonController controller, DragonParameterLoader paramLoader)
        : base(controller, paramLoader)
    {
        _alertParams = paramLoader.Alert;
    }

    protected override void OnEnterState()
    {
        Controller.FightMode(true);
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
        float distance = Vector3.Distance(Controller.transform.position, Controller.Target.transform.position);
        
        if (distance <= Controller.BaseParams.MeleeAttackDistance)
        {
            Machine.TryActivateState<DragonState_MeleeAttack>(true);
            return;
        }
        //
        // float rand = Random.value;
        // if (rand < _alertParams.ChaseProbability)
        // {
        //     Machine.TryActivateState<DragonState_Chase>(true);
        // }
        // else
        // {
        //     Machine.TryActivateState<DragonState_RangedAttack>(true);
        // }
    }

    protected override void OnExitState()
    {
        Controller.Animator.SetBool("IsMove", false);
        Controller.NavMeshAgent.ResetPath();
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

        Vector3 destination = center + rotatedDir * distance;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Controller.NavMeshAgent.SetDestination(hit.position);
            _hasDestination = true;
        }
    }

    private bool Arrived()
    {
        return !Controller.NavMeshAgent.pathPending &&
               Controller.NavMeshAgent.remainingDistance <= Controller.NavMeshAgent.stoppingDistance;
    }
}
