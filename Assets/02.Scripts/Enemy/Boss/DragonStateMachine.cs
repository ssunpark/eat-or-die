using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class DragonStateMachine : NetworkBehaviour
{
    [SerializeField]
    private GameObject _target;
    public GameObject Target => _target;

    [SerializeField]
    private float _speed;

    private Animator _animator;
    public Animator Animator => _animator;

    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    private IEnemyState<DragonStateMachine> _currentState;
    private Dictionary<EBossState, IEnemyState<DragonStateMachine>> _stateDictionary;

    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _stateDictionary = new Dictionary<EBossState, IEnemyState<DragonStateMachine>>
        {
            { EBossState.Idle, new DragonIdleState() },
            { EBossState.Phase1, new DragonPhase1State() },
            // { EBossState.Phase2, new DragonPhase2State() },
            // { EBossState.Phase3, new DragonPhase3State() },
            // { EBossState.Dead, new DragonDeadState() },
        };
        ChangeState(EBossState.Idle);
    }

    public override void FixedUpdateNetwork()
    {
        _currentState?.Update(this, Runner.DeltaTime);
    }

    public void ChangeState(EBossState newState)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        _currentState?.Exit(this);
        _currentState = _stateDictionary[newState];
        _currentState?.Enter(this);
    }

    public void Move(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        // 현재 방향에서 목표 방향을 향한 회전 생성
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 일정한 속도로 회전 (초당 360도 회전, 필요에 따라 조절 가능)
        float rotateSpeed = 360f; // 초당 회전 각도
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Runner.DeltaTime);

        // 이동은 목표 방향 기준으로 계속 직진 (회전값과는 별개)
        transform.position += transform.forward * _speed * Runner.DeltaTime;
    }
}