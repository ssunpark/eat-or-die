using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine;

public class DragonController : NetworkBehaviour, IStateMachineOwner, IAnimationEntryActionNotify,
    IAnimationExitActionNotify
{
    [Header("공격 지점")]
    [SerializeField]
    private Transform _breathPoint;
    public Transform BreathPoint => _breathPoint;

    [SerializeField]
    private Transform _rightPoint;
    public Transform RightPoint => _rightPoint;

    [SerializeField]
    private Transform _leftPoint;
    public Transform LeftPoint => _leftPoint;

    [Header("스킬 오브젝트")]
    [SerializeField]
    private RoarExplosion _roarExplosion;
    public RoarExplosion RoarExplosion => _roarExplosion;

    [SerializeField]
    private GameObject _roarEffect;

    public GameObject RoarEffect => _roarEffect;

    [Header("스킬 오브젝트 (풀링)")]
    [SerializeField]
    private DragonBreathEffect _dragonBreathEffectPrefab;
    public DragonBreathEffect DragonBreathEffectPrefab => _dragonBreathEffectPrefab;

    [SerializeField]
    private LavaProjectile _lavaProjectilePrefab;
    public LavaProjectile LavaProjectile => _lavaProjectilePrefab;

    [SerializeField]
    private LavaFloor _lavaFloorPrefab;
    public LavaFloor LavaFloorPrefab => _lavaFloorPrefab;

    [SerializeField]
    private BloodExplosion _bloodExplosionPrefabPrefab;
    public BloodExplosion BloodExplosionPrefab => _bloodExplosionPrefabPrefab;

    [SerializeField]
    private List<DirectionalProjectile> _directionalProjectiles;
    public List<DirectionalProjectile> DirectionalProjectiles => _directionalProjectiles;

    [Header("연출 오브젝트")]
    [SerializeField]
    private GameObject _phaseEffect;
    public GameObject PhaseEffect => _phaseEffect;

    [Header("감지기")]
    [SerializeField]
    private SightDetector _sightDetector;
    public SightDetector SightDetector => _sightDetector;

    [SerializeField]
    private SightDetector _attackDetector;
    public SightDetector AttackDetector => _attackDetector;

    [Header("테스트")]
    [SerializeField]
    private float _testDamage;
    public bool IsLock;

    private DragonContext _context;
    private DragonStateMachine _stateMachine;

    public Animator Animator { get; private set; }
    public GameObject Target { get; private set; }
    public DragonParameterLoader ParamLoader { get; private set; }
    public DragonObjectPool Pool { get; private set; }

    [Networked]
    private bool _isFightMode { get; set; }

    public bool IsFightMode => _isFightMode;

    [Networked]
    public Vector2 AnimVelocity { get; set; }
    
    [Networked, OnChangedRender(nameof(OnAnimWaitIndexChanged))]
    public int AnimWaitIndex { get; set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        ParamLoader = new DragonParameterLoader();
        Pool = new DragonObjectPool(this);
        _context = new DragonContext(this);
        _stateMachine = new DragonStateMachine(_context);
    }

    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            _context.Movement.NavMeshAgent.enabled = false;
            return;
        }

        _context.OnSpawned();
    }

    private void Update()
    {
        IsLock = _context.Movement.IsLocked;
    }
    
    public override void Render()
    {
        // 네트워크로 복제된 동일 값
        var v = AnimVelocity;
        Animator.SetFloat("XVelocity", v.x);
        Animator.SetFloat("ZVelocity", v.y);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetFightLayerWeight(bool active)
    {
        float weight = active ?  1f : 0f;
        int index = Animator.GetLayerIndex("Fight Layer");
        Animator.SetLayerWeight(index, weight);
        if (active)
        {
            Animator.SetTrigger("Roar");
        }
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _stateMachine.CollectStateMachines(stateMachines);
    }

    public void OnEntryMoment()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (_stateMachine.Machine.ActiveState is IAnimationEntryActionNotify notify)
        {
            notify.OnEntryMoment();
        }
    }

    public void OnActionMoment()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (_stateMachine.Machine.ActiveState is IAnimationActionNotify notify)
        {
            notify.OnActionMoment();
        }
    }

    public void OnExitMoment()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (_stateMachine.Machine.ActiveState is IAnimationExitActionNotify notify)
        {
            notify.OnExitMoment();
        }
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    private void OnAnimWaitIndexChanged()
    {
        Animator.SetInteger("IdleIndex", AnimWaitIndex);
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamage()
    {
        _context.Stats.TakeDamage(_testDamage);
    }
}