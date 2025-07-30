using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Look : DragonSubStateBase
{
    private DragonStateParameterSet.LookParams _lookParams;

    private bool _hasDestination;

    public DragonState_Look(DragonController controller, IParentState parent, DragonStateParameterSet.LookParams lookParams) : base(controller, parent)
    {
        _lookParams = lookParams;
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
        
        if (Machine.StateTime >= _lookParams.LookDuration)
        {
            ParentState.OnSubStateComplete();
            return;
        }

        if (!_hasDestination || Arrived())
        {
            ChooseNewAngleAndDestination();
        }

        Controller.Move(Machine.Runner.DeltaTime); // 이동 + 회전
    }

    protected override void OnExitState()
    {
        Controller.Animator.SetBool("IsMove", false);
        Controller.NavMeshAgent.ResetPath();
    }

    private void ChooseNewAngleAndDestination()
    {
        Vector3 center = Controller.Target.transform.position;
        Vector3 dir = (Controller.transform.position - center).normalized;

        // 랜덤 각도로 이동
        int randomSign = Random.value < 0.5f ? -1 : 1;
        float offsetAngle = randomSign * Random.Range(_lookParams.MinAngleRange, _lookParams.AngleRange);
        Vector3 rotatedDir = Quaternion.Euler(0f, offsetAngle, 0f) * dir;

        float distance = Vector3.Distance(Controller.Target.transform.position, Controller.transform.position)
                         + Random.Range(-_lookParams.WalkRange, _lookParams.WalkRange);

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