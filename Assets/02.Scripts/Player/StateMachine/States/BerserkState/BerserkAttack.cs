using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public class BerserkAttack : ABerserkSubStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    private float _damage;
    private bool _animFinished;
    public BerserkAttack(PlayerFSM controller) : base(controller) { }
    protected override void OnInitialize()
    {
        this.AddTransition(
            Machine.GetState<BerserkChase>(),
            () => _animFinished && _controller.HasStateAuthority
        );
    }
    protected override void OnEnterState()
    {
        _animFinished = false;
        _damage = (_stat.GetStat(EStatType.MeleeDamage) + _stat.GetStat(EStatType.MagicDamage)) * _stat.GetStat(EStatType.TotalDamage);

        _controller.LastAttackTime = Machine.Runner.LocalRenderTime;
        _controller.PlayAnimTrigger(EAnimTrigger.Attack);
    }

    public void OnActionMoment()
    {
        Vector3 attackOrigin = _controller.transform.position + Vector3.up * 0.6f;
        Vector3 direction = _controller.transform.forward;

        if (Physics.Raycast(attackOrigin, direction, out RaycastHit hit, _stat.GetStat(EStatType.AttackRange)))
        {
            if (hit.collider.TryGetComponent(out NetworkObject target))
            {
                if (_controller.HasStateAuthority)
                {
                    _controller.RPC_DealDamage(target, _damage);
                }
            }
        }
    }

    public void OnAnimationFinished()
    {
        _animFinished = true;
    }
}
