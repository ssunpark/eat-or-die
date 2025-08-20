using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Addons.FSM;
using RaycastPro.Detectors;
using UnityEngine;

public class DragonController : NetworkBehaviour, IStateMachineOwner, IAnimationEntryActionNotify,
    IAnimationExitActionNotify, IAttackable
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

    private DragonContext _context;
    private DragonStateMachine _stateMachine;

    public Animator Animator { get; private set; }
    public GameObject Target { get; private set; }
    private Player _targetPlayer;
    private HashSet<GameObject> _targets = new HashSet<GameObject>();
    public DragonParameterLoader ParamLoader { get; private set; }
    public DragonObjectPool Pool { get; private set; }

    public NetworkObject NetworkObject => Object;

    // 네트워크 동기화 값
    [Networked]
    public bool IsFightMode { get; set; }

    [Networked]
    public Vector2 AnimVelocity { get; set; }

    [Networked, OnChangedRender(nameof(OnAnimWaitIndexChanged))]
    public int AnimWaitIndex { get; set; }

    [Networked]
    public TickTimer BreathTimer { get; set; }

    [Networked]
    public float CurrentHeath { get; set; }

    [Header("테스트")]
    public bool Islocked;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        ParamLoader = new DragonParameterLoader();
        Pool = new DragonObjectPool(this);
        _context = new DragonContext(this);
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
        Islocked = _context.Movement.IsLocked;

        if (_targetPlayer?.IsDead ?? false)
        {
            // 다른 타겟으로 변환
            _targets.Remove(Target);
            SetTarget(_targets.FirstOrDefault());
        }
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
        float weight = active ? 1f : 0f;
        int index = Animator.GetLayerIndex("Fight Layer");
        Animator.SetLayerWeight(index, weight);
        if (active)
        {
            Animator.SetTrigger("Roar");
        }
    }

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _stateMachine = new DragonStateMachine(_context);
        _stateMachine?.CollectStateMachines(stateMachines);
    }

    public void OnEntryMoment()
    {
        if (_stateMachine.Machine.ActiveState is IAnimationEntryActionNotify notify)
        {
            notify.OnEntryMoment();
        }
    }

    public void OnActionMoment()
    {
        if (_stateMachine.Machine.ActiveState is IAnimationActionNotify notify)
        {
            notify.OnActionMoment();
        }
    }

    public void OnExitMoment()
    {
        if (_stateMachine.Machine.ActiveState is IAnimationExitActionNotify notify)
        {
            notify.OnExitMoment();
        }
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
        _targetPlayer = Target.GetComponent<Player>();
    }

    private void OnAnimWaitIndexChanged()
    {
        Animator.SetInteger("IdleIndex", AnimWaitIndex);
    }

    public void OnHitLocal(AttackInfo attack)
    {
        if (HasStateAuthority)
        {
            SetTarget(attack.Attacker.gameObject);
            float amount = (attack.MeleeDamage + attack.MagicDamage) * attack.TotalDamageMultiplier *
                           attack.BossDamageMultiplier;
            _context.Stats.TakeDamage(amount);
            ParticleManager.Instance.DamageSpawn(amount, transform.position + Vector3.up, EDamageFloaterType.Damage,
                true);
        }
    }

    public void OnHitStateAuthority(AttackInfo attack)
    {
    }
}