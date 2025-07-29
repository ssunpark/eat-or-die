using UnityEngine;
using UnityEngine.AI;

public class DragonLookState : IEnemyState<DragonStateMachine>
{
    private readonly IParentStateMachine _parent;

    private float _duration = 3f;
    private float _timer;

    private float _walkRange = 5f;   // 앞뒤 이동 거리
    private float _angleRange = 30f; // 좌우 이동 각도
    private float _minAngleRange = 20f; // 좌우 이동 각도

    private bool _hasDestination;

    public bool IsInterruptable => true;

    public DragonLookState(IParentStateMachine parent)
    {
        _parent = parent;
    }

    public void Enter(DragonStateMachine stateMachine)
    {
        Debug.Log("LookState 진입");

        _timer = _duration;
        _hasDestination = false;

        stateMachine.Animator.SetBool("IsMove", true);
    }

    public void Update(DragonStateMachine stateMachine, float dt)
    {
        _timer -= dt;

        if (_timer <= 0f)
        {
            _parent.OnSubStateComplete();
            return;
        }

        if (!_hasDestination || Arrived(stateMachine))
        {
            ChooseNewAngleAndDestination(stateMachine);
        }

        stateMachine.Move(dt); // 이동 + 회전
    }

    public void Exit(DragonStateMachine stateMachine)
    {
        Debug.Log("LookState 종료");
        stateMachine.Animator.SetBool("IsMove", false);
        stateMachine.NavMeshAgent.ResetPath();
    }
    
    private void ChooseNewAngleAndDestination(DragonStateMachine stateMachine)
    {
        Vector3 center = stateMachine.Target.transform.position;
        Vector3 dir = (stateMachine.transform.position - center).normalized;

        // 랜덤 각도로 이동
        int randomSign = Random.value < 0.5f ? -1 : 1;
        float offsetAngle = randomSign * Random.Range(_minAngleRange, _angleRange);
        Vector3 rotatedDir = Quaternion.Euler(0f, offsetAngle, 0f) * dir;
        
        float distance = Vector3.Distance(stateMachine.Target.transform.position, stateMachine.transform.position)
            + Random.Range(-_walkRange, _walkRange);
        
        Vector3 destination = center + rotatedDir * distance;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            stateMachine.NavMeshAgent.SetDestination(hit.position);
            _hasDestination = true;
        }
    }

    private bool Arrived(DragonStateMachine stateMachine)
    {
        return !stateMachine.NavMeshAgent.pathPending &&
               stateMachine.NavMeshAgent.remainingDistance <= stateMachine.NavMeshAgent.stoppingDistance;
    }
}
