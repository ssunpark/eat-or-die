using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public class BerserkAttack : ABerserkSubStateBase
{
    public BerserkAttack(PlayerFSM fsm) : base(fsm) {
        AnimState = "Attack";
        _positionOffset = new Vector3(0f, 0.2f, 0.5f);
    }
    private Vector3 _positionOffset;
    private float _meleeDamage;
    private float _magicDamage;
    private float _knockbackStrength = 5f;
    private float _hitStunLength = 0.5f;
    private float _totalDamageMultiplier = 1f;
    private float _bossDamageMultiplier = 1f;
    private float _attackSpeed = 1f;
    private float _animationTime;
    private Collider[] _hitsColliders = new Collider[8];
    protected override void OnEnterState()
    {
        LazySet();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        _meleeDamage = _stat.GetStat(EStatType.MeleeDamage);
        _magicDamage = _stat.GetStat(EStatType.MagicDamage);
        //_knockbackStrength = _stat.GetStat(EStatType.KnockbackStrength);
        //_hitStunLength = _stat.GetStat(EStatType.HitStunLength);
        _totalDamageMultiplier = _stat.GetStat(EStatType.TotalDamage);
        _bossDamageMultiplier = _stat.GetStat(EStatType.BossDamage);

        _fsm.LastAttackTime = Machine.Runner.LocalRenderTime;
        _attackSpeed = _stat?.GetStat(EStatType.AttackSpeed) ?? 1f;
        Anim.SetFloat("AttackSpeed", _attackSpeed);
        float baseClipLength = _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState];
        _animationTime = Mathf.Max(baseClipLength / _attackSpeed, 0.06f);
    }

    protected override void OnEnterStateRender()
    {
        LazySet();
        _attackSpeed = _stat?.GetStat(EStatType.AttackSpeed) ?? 1f;
        Anim.SetFloat("AttackSpeed", _attackSpeed);
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    public override void OnActionMoment()
    {
        Vector3 attackOrigin = _fsm.transform.position + _fsm.transform.rotation * _positionOffset;
        Vector3 direction = KCC.GetLookRotation();
        Vector2 dir2d = new Vector2(direction.x, direction.z);
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
        Debug.Log($"[PlayerAttackState] Performing attack of type: {attackType} at origin: {attackOrigin} with direction: {direction}");
        string projectileKey = _fsm.PlayerNetworkObject.ItemHolder.ProjectileKey;
        switch (attackType)
        {
            case EAttackType.MeleeWeapon:
                PerformMeleeAttack(attackOrigin, attackInfo);
                break;
            case EAttackType.RangeWeapon:
                if (_fsm.HasStateAuthority)
                PerformRangedAttack(attackOrigin, dir2d, projectileKey, attackInfo);
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
        Debug.Log($"[PlayerAttackState] Performing ranged attack with projectile key: {projectileKey}");
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

    protected override void OnExitState()
    {
        _fsm.HitTargets.Clear();
    }
    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _animationTime)
        {
            Machine.ForceActivateState<BerserkChase>();
            return;
        }
    }
}
