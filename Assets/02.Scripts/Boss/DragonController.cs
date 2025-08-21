using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using Fusion;
using Fusion.Addons.FSM;
using INab.Common;
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

    [SerializeField]
    private InteractiveEffect _dissolve;
    public InteractiveEffect Dissolve => _dissolve;

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
    public Player TargetPlayer { get; private set; }
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

    private bool _hit;
    private bool _isDead;
    public bool IsDead { get => _isDead; set => _isDead = value; }
    private float _deadTimer;

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

    public override void FixedUpdateNetwork()
    {
        if (_stateMachine.Machine.ActiveState is not DragonState_Idle && (TargetPlayer == null || TargetPlayer.IsDead))
        {
            _stateMachine.Machine.ForceActivateState<DragonState_Idle>();
        }
        
        if (_hit && _context.Stats.CurrentHP <= 0 && !_isDead)
        {
            _stateMachine.Machine.ForceActivateState<DragonState_Death>(true);
            _hit = false;
        }

        if (!_isDead)
            return;

        _deadTimer += Time.deltaTime;
        if (_deadTimer >= _dissolve.duration)
        {
            Runner.Despawn(Object);
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
            MasterAudio.PlaySound3DAtTransform("Roar", transform);
        }
    }
    
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Death()
    {
        int index = Animator.GetLayerIndex("Fight Layer");
        Animator.SetLayerWeight(index, 0);
        Animator.SetTrigger("Death");
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
        TargetPlayer = target?.GetComponent<Player>();
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

            _hit = true;
        }
    }

    public void OnHitStateAuthority(AttackInfo attack)
    {
    }
}