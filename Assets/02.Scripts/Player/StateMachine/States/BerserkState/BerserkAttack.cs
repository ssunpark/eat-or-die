using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public class BerserkAttack : ABerserkSubStateBase, IAnimationActionNotify
{
    private float _damage;
    private bool _animFinished;
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
        if (_stat == null)
        {
            _stat = _fsm.PlayerNetworkObject.Stat;
        }
        if (_resource == null)
        {
            _resource = _fsm.PlayerNetworkObject.Resource;
        }

        if (_stat == null || _resource == null)
        {
            Debug.LogError("PlayerMoveState: Stat or Resource is null. Cannot enter state.");
            return;
        }
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
        _attackSpeed = _stat?.GetStat(EStatType.AttackSpeed) ?? 1f;
        Anim.SetFloat("AttackSpeed", _attackSpeed);
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    public void OnActionMoment()
    {
        Vector3 attackOrigin = _fsm.transform.position + _fsm.transform.rotation * _positionOffset;
        Vector3 direction = _fsm.transform.forward;

        int result = Machine.Runner.GetPhysicsScene().OverlapSphere(attackOrigin, _stat.GetStat(EStatType.AttackRange), _hitsColliders,
                    _fsm.attackableLayerMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < result; i++)
        {
            IAttackable target = _hitsColliders[i].GetComponent<IAttackable>();

            // If no enemy has been hit or this target has already been hit, we continue.
            if (target == null || hitTargets.Contains(target.NetworkObject))
                continue;

            AttackInfo attackState = new AttackInfo()
            {
                MeleeDamage = _meleeDamage,
                MagicDamage = _magicDamage,
                TotalDamageMultiplier = _totalDamageMultiplier,
                BossDamageMultiplier = _bossDamageMultiplier,
                KnockbackVector = _fsm.transform.forward * _knockbackStrength,
                HitRecoveryTime = _hitStunLength,
            };
            target.OnHitLocal(attackState, _fsm.PlayerNetworkObject?.Object);

            if (i >= hitTargets.Count)
                hitTargets.Add(target.NetworkObject);
            else
                hitTargets.Set(i, target.NetworkObject);
        }

    }

    protected override void OnFixedUpdate()
    {
        KCC.Move(Vector3.zero);

        if (Machine.StateTime >= _animationTime)
        {
            Machine.ForceActivateState<BerserkChase>();
            return;
        }
    }
}
