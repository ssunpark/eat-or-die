using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine;

public class DragonController : NetworkBehaviour, IStateMachineOwner
{
    [SerializeField]
    private Transform _breathPoint;
    
    public Transform BreathPoint => _breathPoint;

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
    
    private DragonContext _context;
    private DragonStateMachine _stateMachine;
    
    public Animator Animator { get; private set; }
    public GameObject Target { get; private set; }
    public DragonParameterLoader ParamLoader { get; private set; }
    public DragonObjectPool Pool { get; private set; }

    [Networked]
    private bool _isFightMode { get; set; }

    public bool IsFightMode => _isFightMode;

    private void Awake()
    {
        _context = new DragonContext(this);
        _stateMachine = new DragonStateMachine(_context);
        Pool = new DragonObjectPool(this);
        ParamLoader = new DragonParameterLoader();
        Animator = GetComponent<Animator>();
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetFightLayerWeight(float weight)
    {
        int index = _context.Animator.GetLayerIndex("Fight Layer");
        _context.Animator.SetLayerWeight(index, weight);
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _stateMachine.CollectStateMachines(stateMachines);
    }

    public void AnimationStartEvent()
    {
        _context.Movement.Lock();
    }

    public void AnimationEndEvent()
    {
        _context.Movement.Unlock();
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamage()
    {
        _context.Stats.TakeDamage(_testDamage);
    }
}