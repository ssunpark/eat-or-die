using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using static DG.Tweening.DOTweenAnimation;
public class PlayerAttackState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerAttackState(PlayerFSM controller) : base(controller)
    {
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
    private bool _animationFinished;
    private float _attackSpeed = 1f;
    private float _animationTime;
    [Networked, Capacity(8)]
    public NetworkLinkedList<NetworkObject> hitTargets => default;
    private Collider[] _hitsColliders = new Collider[8];
    protected override void OnEnterState()
    {
        _animationFinished = false;
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
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
        _meleeDamage = _stat?.GetStat(EStatType.MeleeDamage)??1;
        _magicDamage = _stat?.GetStat(EStatType.MagicDamage)??0;
        //_knockbackStrength = _stat.GetStat(EStatType.KnockbackStrength);
        //_hitStunLength = _stat.GetStat(EStatType.HitStunLength);
        _totalDamageMultiplier = _stat?.GetStat(EStatType.TotalDamage)??1;
        _bossDamageMultiplier = _stat?.GetStat(EStatType.BossDamage)??1;
        
        _fsm.LastAttackTime = Machine.Runner.LocalRenderTime;
        _attackSpeed = _stat?.GetStat(EStatType.AttackSpeed)??1f;
        Anim.SetFloat("AttackSpeed", _attackSpeed);
        float baseClipLength = _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState];
        _animationTime = Mathf.Max(baseClipLength / _attackSpeed,0.06f);
    }


    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnExitState()
    {
        hitTargets.Clear();
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
            Machine.ForceActivateState<PlayerIdleState>();
            return;
        }
    }
}
