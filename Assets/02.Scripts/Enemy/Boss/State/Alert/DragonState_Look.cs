using UnityEngine;
using UnityEngine.AI;
using Fusion.Addons.FSM;

public class DragonState_Look : DragonSubStateBase
{
    private float _lookDuration = 5f;

    private float _walkRange = 5f;      // 앞뒤 이동 거리
    private float _angleRange = 30f;    // 좌우 이동 각도
    private float _minAngleRange = 20f; // 좌우 이동 각도

    private bool _hasDestination;

    public DragonState_Look(DragonStateMachine machine, IParentStateMachine parentMachine) : base(machine, parentMachine)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("LookState 진입");

        _hasDestination = false;

        StateMachine.Animator.SetBool("IsMove", true);
    }

    protected override void OnFixedUpdate()
    {
        if (StateMachine.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }
        
        if (Machine.StateTime >= _lookDuration)
        {
            ParentStateMachine.OnSubStateComplete();
            return;
        }

        if (!_hasDestination || Arrived())
        {
            ChooseNewAngleAndDestination();
        }

        StateMachine.Move(Machine.Runner.DeltaTime); // 이동 + 회전
    }

    protected override void OnExitState()
    {
        Debug.Log("LookState 종료");
        StateMachine.Animator.SetBool("IsMove", false);
        StateMachine.NavMeshAgent.ResetPath();
    }

    private void ChooseNewAngleAndDestination()
    {
        Vector3 center = StateMachine.Target.transform.position;
        Vector3 dir = (StateMachine.transform.position - center).normalized;

        // 랜덤 각도로 이동
        int randomSign = Random.value < 0.5f ? -1 : 1;
        float offsetAngle = randomSign * Random.Range(_minAngleRange, _angleRange);
        Vector3 rotatedDir = Quaternion.Euler(0f, offsetAngle, 0f) * dir;

        float distance = Vector3.Distance(StateMachine.Target.transform.position, StateMachine.transform.position)
                         + Random.Range(-_walkRange, _walkRange);

        Vector3 destination = center + rotatedDir * distance;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            StateMachine.NavMeshAgent.SetDestination(hit.position);
            _hasDestination = true;
        }
    }

    private bool Arrived()
    {
        return !StateMachine.NavMeshAgent.pathPending &&
               StateMachine.NavMeshAgent.remainingDistance <= StateMachine.NavMeshAgent.stoppingDistance;
    }
}