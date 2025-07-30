using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public class BerserkAttack : ABerserkSubStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    private float _attackDelay;
    private float _damage;
    public BerserkAttack(PlayerController controller) : base(controller) { }

    protected override void OnEnterState()
    {
        _damage = (_stat.GetStat(EStatType.MeleeDamage) + _stat.GetStat(EStatType.MagicDamage)) * _stat.GetStat(EStatType.TotalDamage);

        float speed = _controller.Stat.GetStat(EStatType.AttackSpeed);
        _attackDelay = 1f / Mathf.Max(speed, 0.01f);

        _controller.LastAttackTime = Machine.Runner.LocalRenderTime;
        _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Attack);

    }

    public void OnActionMoment()
    {
        Vector3 attackOrigin = _controller.transform.position + Vector3.up * 0.5f;
        Vector3 direction = _controller.transform.forward;

        if (Physics.Raycast(attackOrigin, direction, out RaycastHit hit, _stat.GetStat(EStatType.AttackRange)))
        {
            if (hit.collider.TryGetComponent(out NetworkObject target))
            {
                _controller.RPC_DealDamage(target, _damage);
            }
        }
    }

    public void OnAnimationFinished()
    {
        Machine.ForceActivateState<BerserkChase>();
    }
}
