using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.AI;

public class DragonController : NetworkBehaviour, IStateMachineOwner
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
    
    private StateMachine<DragonStateBase> _dragonStateMachine;

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

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _dragonStateMachine = new StateMachine<DragonStateBase>("DragonStateMachine", new DragonState_Idle(this), new DragonState_Alert(this));
        
        stateMachines.Add(_dragonStateMachine);
    }
}