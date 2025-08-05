using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AI;

public class DragonController : NetworkBehaviour, IStateMachineOwner
{
    private const string ANIMATION_LAYER_FIGHT = "Fight Layer";
    [SerializeField]
    private Transform _breathPoint;
    public Transform BreathPoint => _breathPoint;

    [SerializeField]
    private DragonBreathHitBox _breathHitBoxPrefab;
    private Pool<DragonBreathHitBox> _breathHitBoxPool;
    public Pool<DragonBreathHitBox> BreathHitBoxPool => _breathHitBoxPool;
    
    [SerializeField]
    private BreathParticle _breathParticle;
    private Pool<BreathParticle> _breathParticlePool;
    public Pool<BreathParticle> BreathParticlePool => _breathParticlePool;
    
    [SerializeField]
    private LavaProjectile _lavaProjectilePrefab;
    private Pool<LavaProjectile> _lavaProjectilePool;
    public Pool<LavaProjectile> LavaProjectilePool => _lavaProjectilePool;
    
    [SerializeField]
    private LavaFloor _lavaFloorPrefab;
    private Pool<LavaFloor> _lavaFloorPool;
    public Pool<LavaFloor> LavaFloorPool => _lavaFloorPool;

    [SerializeField]
    private GameObject _target;
    public GameObject Target => _target;
    
    [Networked]
    private bool _isFightMode {get; set;}

    private Animator _animator;
    public Animator Animator => _animator;

    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    private bool _isLocked = false;
    public bool IsLocked => _isLocked;

    private Vector2 _smoothedVelocity = Vector2.zero;

    private SightDetector _sightDetector;
    public SightDetector SightDetector => _sightDetector;

    private DragonStateMachine _dragonStateMachine;

    private DragonStateParameterSet.BaseParams _baseParams => _dragonStateMachine?.ParamLoader?.Base;

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _dragonStateMachine.CollectStateMachines(stateMachines);
    }

    private void Awake()
    {
        _dragonStateMachine = new DragonStateMachine(this);
        _sightDetector = GetComponentInChildren<SightDetector>();
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        // 풀링
        GameObject lavaPool = new GameObject("LavaPool");
        _breathHitBoxPool = Pool.Create(_breathHitBoxPrefab, 3, transform).NonLazy();
        _breathParticlePool = Pool.Create(_breathParticle, 3, transform).NonLazy();
        _lavaProjectilePool = Pool.Create(_lavaProjectilePrefab, 0, lavaPool.transform);
        _lavaFloorPool = Pool.Create(_lavaFloorPrefab, 0, lavaPool.transform);
    }

    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            return;
        }

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _navMeshAgent.speed = _baseParams.MoveSpeed;
        _navMeshAgent.angularSpeed = _baseParams.RotationSpeed;
        
        FightMode(_isFightMode);
    }

    public void Lock()
    {
        _isLocked = true;
        if (!_navMeshAgent.enabled)
        {
            return;
        }

        _navMeshAgent.ResetPath();
    }

    public void Unlock()
    {
        _isLocked = false;
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public void Move(float dt)
    {
        if (_navMeshAgent.pathPending)
            return;

        // 1. 위치 이동
        Vector3 next = _navMeshAgent.nextPosition;
        transform.position = next;

        // 2. 회전 처리
        Vector3 direction;

        if (Target != null)
        {
            direction = Target.transform.position - transform.position;
        }
        else
        {
            direction = _navMeshAgent.steeringTarget - transform.position;
        }

        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                _navMeshAgent.angularSpeed * dt
            );
        }

        // 3. 애니메이션 파라미터 (보간 포함)
        Vector3 localVelocity = transform.InverseTransformDirection(_navMeshAgent.desiredVelocity);
        Vector2 targetVelocity = new Vector2(
            localVelocity.x,
            localVelocity.z
        );

        // Lerp 보간
        _smoothedVelocity = Vector2.Lerp(_smoothedVelocity, targetVelocity, _baseParams.AnimSmoothSpeed * dt);

        _animator.SetFloat("XVelocity", _smoothedVelocity.x);
        _animator.SetFloat("ZVelocity", _smoothedVelocity.y);
    }

    public void MaintainDistanceAndLookAtTarget(float dt, float desiredDistance)
    {
        if (Target == null)
            return;

        Vector3 direction = transform.position - Target.transform.position;
        direction.y = 0f;

        float currentDistance = direction.magnitude;
        if (currentDistance < 0.01f)
            return;

        // 1. 회전: 타겟을 바라봄
        Quaternion targetRot = Quaternion.LookRotation(-direction.normalized); // 반대 방향 바라보기 아님
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            _navMeshAgent.angularSpeed / 2f * dt
        );

        // 2. 거리 유지: 너무 가까우면 뒤로 이동
        if (currentDistance < desiredDistance)
        {
            Vector3 backDir = transform.forward * -1f;
            float moveDistance = (desiredDistance - currentDistance);
            Vector3 newPosition = transform.position + backDir * moveDistance;

            // y 고정 (수직 이동 방지)
            newPosition.y = transform.position.y;

            transform.position = Vector3.MoveTowards(transform.position, newPosition, _navMeshAgent.speed * dt);
        }
    }

    public void FightMode(bool active)
    {
        int layerIndex = _animator.GetLayerIndex(ANIMATION_LAYER_FIGHT);
        float weight = active ? 1f : 0f;
        _animator.SetLayerWeight(layerIndex, weight);
        
        if (HasStateAuthority)
        {
            _isFightMode = active;
            RPC_SetFightLayerWeight(weight);
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetFightLayerWeight(float weight)
    {
        int layerIndex = _animator.GetLayerIndex(ANIMATION_LAYER_FIGHT);
        _animator.SetLayerWeight(layerIndex, weight);
    }

    public void SetSightDetector(float fullAwarenessRadius, float detectRadius, float detectAngle)
    {
        _sightDetector.fullAwareness = fullAwarenessRadius;
        _sightDetector.minRadius = fullAwarenessRadius;
        _sightDetector.Radius = detectRadius;
        _sightDetector.angleX = detectAngle;
    }
    
    public void SetDestination(Vector3 position)
    {
        if (!_navMeshAgent.enabled)
            return;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(position);
    }

    public void ResetNavMeshAgent()
    {
        _navMeshAgent.ResetPath();
        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.nextPosition = transform.position;
    }

    public void SetNavMeshAgentMoveData(float moveSpeed, float angularSpeed)
    {
        _navMeshAgent.speed = moveSpeed;
        _navMeshAgent.angularSpeed = angularSpeed;
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (_baseParams == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _dragonStateMachine.ParamLoader.Prepare.MinDistanceToFinishPrepare);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _baseParams.MeleeAttackDistance);

        float angle = _baseParams.FOVAngle;
        Vector3 left = Quaternion.Euler(0, -angle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, angle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, left * _baseParams.DetectRadius);
        Gizmos.DrawRay(transform.position, right * _baseParams.DetectRadius);
#endif
    }
}