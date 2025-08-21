using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public class BerserkAttack : ABerserkSubStateBase
{
    public BerserkAttack(PlayerFSM fsm) : base(fsm) {
        AnimState = "Attack";
        _positionOffset = new Vector3(0f, 0.7f, 0.3f);
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

        LazySet();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        float baseClipLength = _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState];
        _animationTime = Mathf.Max(baseClipLength / _stat.GetStat(EStatType.AttackSpeed), 0.06f);
    }
    private Vector3 _direction;

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();

        LazySet();
        _fsm.LastAttackTime = Machine.Runner.LocalRenderTime;

        Anim.SetFloat("AttackSpeed", _stat.GetStat(EStatType.AttackSpeed));
        _isRenderInitialized = true;

        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
        _direction = _fsm.transform.forward;
    }

    protected override void OnExitState()
    {
        _fsm.HitTargets.Clear();
        _isRenderInitialized = false;
    }

    protected override int GetNetworkDataWordCount() => 1;
    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;
        if (Machine.StateTime >= _animationTime && _isRenderInitialized)
        {
            Machine.ForceActivateState<BerserkChase>();
            return;
        }
        else
        {
            KCC.Move(Vector3.zero);
        }
    }

    public override void OnActionMoment()
    {
        Vector3 attackOrigin = _fsm.transform.position + _fsm.transform.rotation * _positionOffset;
        EAttackType attackType = _fsm.PlayerNetworkObject.ItemHolder.AttackType;
        float meleeDamage = _stat.GetStat(EStatType.MeleeDamage);
        float magicDamage = _stat.GetStat(EStatType.MagicDamage);
        float totalDamageMultiplier = _stat.GetStat(EStatType.TotalDamage);
        float bossDamageMultiplier = _stat.GetStat(EStatType.BossDamage);

        AttackInfo attackInfo = new()
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
        PlayAttackVfx(EAttackPhase.Swing, attackOrigin);
        projectile.Initialize(
            attackInfo: attackInfo,
            layerMask: _fsm.BerserkLayerMask
        );
    }
    private void PerformMeleeAttack(Vector3 attackOrigin, AttackInfo attackInfo)
    {
        int result = Machine.Runner.GetPhysicsScene().OverlapSphere(attackOrigin, _stat.GetStat(EStatType.AttackRange), _hitsColliders,
                            _fsm.BerserkLayerMask, QueryTriggerInteraction.Collide);

        PlayAttackVfx(EAttackPhase.Swing, attackOrigin);
        for (int i = 0; i < result; i++)
        {
            IAttackable target = _hitsColliders[i].GetComponent<IAttackable>();

            // If no enemy has been hit or this target has already been hit, we continue.
            if (target == null || _fsm.HitTargets.Contains(target.NetworkObject) || target.NetworkObject == _fsm.PlayerNetworkObject?.Object)
            {
                continue;
            }

            PlayAttackVfx(EAttackPhase.Hit, _hitsColliders[i].transform.position + (Vector3.up * 0.7f));
            target.OnHitLocal(attackInfo);

            if (i >= _fsm.HitTargets.Count)
                _fsm.HitTargets.Add(target.NetworkObject);
            else
                _fsm.HitTargets.Set(i, target.NetworkObject);
        }
    }

    private void PlayAttackVfx(EAttackPhase phase, Vector3 pos)
    {
        var go = _fsm.ItemHolder?.HeldItemObject;
        string key = null;

        if (go == null)
        {
            key = $"{phase.ToString()}_Unarmed";
        }
        else if (go.TryGetComponent<IAttackVfxProvider>(out var vfx))
        {
            key = vfx.GetEffectKey(phase);

        }
        if (string.IsNullOrEmpty(key))
            key = $"{phase.ToString()}_Default";

        var rot = Quaternion.LookRotation(_direction);


        Debug.Log($"[PlayerAttackState] Playing attack VFX: {key} at position: {pos}, rotation: {rot}");
        ParticleManager.Instance.PlayByKeyLocal(key, pos, rot);
    }
}
