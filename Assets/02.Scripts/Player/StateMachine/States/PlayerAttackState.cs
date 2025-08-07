using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public unsafe class PlayerAttackState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerAttackState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Attack";
        _positionOffset = new Vector3(0f, 0.2f, 0.5f);
        StateId = (int)EPlayerState.Attack;
    }
    private Vector3 _positionOffset;
    private float _knockbackStrength = 5f;
    private float _hitStunLength = 0.5f;
    private float _attackSpeed = 1f;
    private float _animationTime;
    private bool _isRenderInitialized = false;

    private Collider[] _hitsColliders = new Collider[8];
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        float baseClipLength = _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState];
        _animationTime = Mathf.Max(baseClipLength / _attackSpeed, 0.06f);
    }
    private Vector3 _direction;

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        _fsm.LastAttackTime = Machine.Runner.LocalRenderTime;

        Anim.SetFloat("AttackSpeed", _attackSpeed);
        _isRenderInitialized = true;

        if (_fsm.HasInputAuthority || _fsm.HasStateAuthority)
        {
            _direction = GetMouseDirection(); 
        }
    }

    protected override void OnExitState()
    {
        _fsm.HitTargets.Clear();
        _isRenderInitialized = false;
    }

    protected override int GetNetworkDataWordCount() => 1;
    protected override void OnFixedUpdateState()
    {

        if (Machine.StateTime >= _animationTime && _isRenderInitialized)
        {
            RequestActivateState();
            return;
        }
    }

    protected override void PostFixedUpdate()
    {
        if (Machine.StateTime <= _animationTime)
        {
            KCC.SetLookRotation(Quaternion.LookRotation(_direction));
            KCC.Move(Vector3.zero);
        }
    }
    protected override void ReadNetworkData(int* ptr)
    {
        float* floatPtr = (float*)ptr;
        _animationTime = floatPtr[0];
    }

    protected override void WriteNetworkData(int* ptr)
    {
        float* floatPtr = (float*)ptr;
        floatPtr[0] = _animationTime;
    }

    private Vector3 GetMouseDirection()
    {
        Vector3 dir = _fsm.CurrentInput.mousePosition - _fsm.transform.position;
        var normalizedDir = dir.normalized;
        normalizedDir.y = 0;
        return normalizedDir;
    }
    public void OnActionMoment()
    {
        Vector3 attackOrigin = _fsm.transform.position + _fsm.transform.rotation * _positionOffset;
        EAttackType attackType = _fsm.PlayerNetworkObject.ItemHolder.AttackType;
        float meleeDamage = _stat.GetStat(EStatType.MeleeDamage);
        float magicDamage = _stat.GetStat(EStatType.MagicDamage);
        float totalDamageMultiplier = _stat.GetStat(EStatType.TotalDamage);
        float bossDamageMultiplier = _stat.GetStat(EStatType.BossDamage);

        AttackInfo attackInfo = new AttackInfo()
        {
            MeleeDamage = meleeDamage,
            MagicDamage = magicDamage,
            TotalDamageMultiplier = totalDamageMultiplier,
            BossDamageMultiplier = bossDamageMultiplier,
            KnockbackVector = _fsm.transform.forward * _knockbackStrength,
            HitRecoveryTime = _hitStunLength,
            Attacker = _fsm.PlayerNetworkObject.Object
        };
        string projectileKey = _fsm.PlayerNetworkObject.ItemHolder.ProjectileKey;
        switch (attackType)
        {
            case EAttackType.MeleeWeapon:
                PerformMeleeAttack(attackOrigin, attackInfo);
                break;
            case EAttackType.RangeWeapon:
                if (_fsm.HasStateAuthority)
                    PerformRangedAttack(attackOrigin, _direction, projectileKey, attackInfo);
                break;
            default:
                Debug.LogWarning($"[PlayerAttackState] Unsupported attack type: {attackType}");
                break;
        }

    }
    private void PerformRangedAttack(Vector3 attackOrigin, Vector3 direction, string projectileKey = "", AttackInfo attackInfo = default)
    {
        if (string.IsNullOrEmpty(projectileKey))
        {
            Debug.LogWarning("[PlayerAttackState] ProjectileKey is null or empty. Using default.");
            projectileKey = "DefaultProjectile";
        }
        GameObject projectilePrefab = ProjectileManager.Instance.GetProjectile(projectileKey);
        if (projectilePrefab == null)
        {
            Debug.LogError($"[PlayerAttackState] Cannot find projectile prefab with key: {projectileKey}");
            return;
        }
        Quaternion rotation = Quaternion.LookRotation(direction);
        var projectile = Machine.Runner.Spawn(projectilePrefab, attackOrigin, rotation, PlayerRef.None).GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("[PlayerAttackState] Spawned object does not have Projectile component.");
            return;
        }

        projectile.Initialize(
            attackInfo: attackInfo
        );
    }
    private void PerformMeleeAttack(Vector3 attackOrigin, AttackInfo attackInfo)
    {
        int result = Machine.Runner.GetPhysicsScene().OverlapSphere(attackOrigin, _stat.GetStat(EStatType.AttackRange), _hitsColliders,
                            _fsm.attackableLayerMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < result; i++)
        {
            IAttackable target = _hitsColliders[i].GetComponent<IAttackable>();

            // If no enemy has been hit or this target has already been hit, we continue.
            if (target == null || _fsm.HitTargets.Contains(target.NetworkObject) || target.NetworkObject == _fsm.PlayerNetworkObject?.Object)
            {
                continue;
            }

            
            target.OnHitLocal(attackInfo);

            if (i >= _fsm.HitTargets.Count)
                _fsm.HitTargets.Add(target.NetworkObject);
            else
                _fsm.HitTargets.Set(i, target.NetworkObject);
        }
    }
}
