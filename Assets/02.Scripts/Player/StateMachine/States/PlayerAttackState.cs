using UnityEngine;
using Fusion;
using Fusion.Addons.FSM;
public class PlayerAttackState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerAttackState(PlayerController controller): base(controller)
    {
        StateId = (int)EPlayerState.Attack;
    }
    private float _damage;

    protected override void OnEnterState()
    {
        float attackSpeed = _stat.GetStat(EStatType.AttackSpeed);
        _damage = (_stat.GetStat(EStatType.MeleeDamage) + _stat.GetStat(EStatType.MagicDamage))*_stat.GetStat(EStatType.TotalDamage);

        _controller.LastAttackTime = Machine.Runner.LocalRenderTime;

        if (_controller.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Attack);
        }
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
        // Todo: 광폭화 상태라면 광폭화로 전환할 것
        Machine.ForceActivateState<PlayerIdleState>();
    }
}
