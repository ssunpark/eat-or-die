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
        if(!_fsm.HasStateAuthority) return;
        Vector3 attackOrigin = _fsm.transform.position + _fsm.transform.rotation * _positionOffset;
        Vector3 direction = _fsm.transform.forward;

        int result = Machine.Runner.GetPhysicsScene().OverlapSphere(attackOrigin, _stat.GetStat(EStatType.AttackRange), _hitsColliders,
                    _fsm.BerserkLayerMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < result; i++)
        {
            IAttackable target = _hitsColliders[i].GetComponent<IAttackable>();
            Debug.Log($"BerserkAttack - {target.NetworkObject.InputAuthority} detected");

            if (target == null || _fsm.HitTargets.Contains(target.NetworkObject) || target.NetworkObject == _fsm.PlayerNetworkObject?.Object)
            {
                Debug.Log($"BerserkAttack - {target.NetworkObject.InputAuthority} is not attackable or already hit or is the player itself.");
                continue;
            }

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
            Debug.Log($"BerserkAttack - {target.NetworkObject.InputAuthority} hit with damage: {attackState.MeleeDamage}");
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
