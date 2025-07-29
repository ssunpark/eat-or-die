using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class DragonStateMachine : NetworkBehaviour
{
    private const string ANIMATION_LAYER_FIGHT = "Fight Layer";
    
    [SerializeField]
    private GameObject _target;
    public GameObject Target => _target;

    private Animator _animator;
    public Animator Animator => _animator;

    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    
    private bool _isLocked = false;
    public bool IsLocked => _isLocked;
    public event Action OnUnlock;
    
    private Vector2 _smoothedVelocity = Vector2.zero;
    [SerializeField]
    private float _animSmoothSpeed = 1f;

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
            { EBossState.Alert, new DragonAlertState() },
            // { EBossState.Attack, new DragonPhase1State() },
            // { EBossState.Spell, new DragonPhase2State() },
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
        if (NavMeshAgent.pathPending) return;

        // 1. 위치 이동
        Vector3 next = NavMeshAgent.nextPosition;
        transform.position = next;

        // 2. 회전 처리
        Vector3 direction;

        if (Target != null)
        {
            direction = Target.transform.position - transform.position;
        }
        else
        {
            direction = NavMeshAgent.steeringTarget - transform.position;
        }

        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                NavMeshAgent.angularSpeed * dt
            );
        }
        
        // 3. 애니메이션 파라미터 (보간 포함)
        Vector3 localVelocity = transform.InverseTransformDirection(NavMeshAgent.desiredVelocity);
        Vector2 targetVelocity = new Vector2(
            Mathf.Clamp(localVelocity.x / NavMeshAgent.speed, -1f, 1f),
            Mathf.Max(0f, localVelocity.z / NavMeshAgent.speed)
        );

        // Lerp 보간
        _smoothedVelocity = Vector2.Lerp(_smoothedVelocity, targetVelocity, _animSmoothSpeed * dt);

        _animator.SetFloat("XVelocity", _smoothedVelocity.x);
        _animator.SetFloat("ZVelocity", _smoothedVelocity.y);
    }

    public void FightMode(bool active)
    {
        int layerIndex = _animator.GetLayerIndex(ANIMATION_LAYER_FIGHT);
        float targetWeight = active ? 1f : 0f;
        _animator.SetLayerWeight(layerIndex, targetWeight);
    }
    
    public void Lock() => _isLocked = true;

    public void Unlock()
    {
        _isLocked = false;
        OnUnlock?.Invoke();
    }
}