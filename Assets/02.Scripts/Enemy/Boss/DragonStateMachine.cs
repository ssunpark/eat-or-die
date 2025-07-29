using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class DragonStateMachine : NetworkBehaviour
{
    [SerializeField]
    private GameObject _target;
    public GameObject Target => _target;

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
            GetComponent<NavMeshAgent>().enabled = false;
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
            // { EBossState.Phase1, new DragonPhase1State() },
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
        if (!HasStateAuthority) return;

        if (_currentState != null && !_currentState.IsInterruptable)
        {
            Debug.Log("현재 상태는 인터럽트 불가");
            return;
        }

        _currentState?.Exit(this);
        _currentState = _stateDictionary[newState];
        _currentState?.Enter(this);
    }
    
    public void ForceChangeState(EBossState newState)
    {
        _currentState?.Exit(this);
        _currentState = _stateDictionary[newState];
        _currentState?.Enter(this);
    }
    
    public void Move(float dt)
    {
        if (_navMeshAgent.pathPending) return;

        Vector3 next = _navMeshAgent.nextPosition;
        Vector3 current = transform.position;
        Vector3 direction = next - current;
        direction.y = 0f;

        // 회전
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                _navMeshAgent.angularSpeed * dt
            );
        }

        // 위치 이동
        transform.position = next;
    }
}