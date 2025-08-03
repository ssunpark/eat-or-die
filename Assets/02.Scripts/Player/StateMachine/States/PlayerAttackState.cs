using UnityEngine;
using Fusion;
using Fusion.Addons.FSM;
public class PlayerAttackState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerAttackState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Attack;
    }
    private float _damage;
    private bool _animationFinished;
    bool hasFinishedAnimation => _animationFinished;
    bool hasMoveInput => _controller.GetInput(out NetworkInputData input) && input.direction.sqrMagnitude > 0.01f;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Move,
            () => hasFinishedAnimation && hasMoveInput
        );

        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => hasFinishedAnimation && !hasMoveInput
        );
    }

    protected override void OnEnterState()
    {
        _animationFinished = false;
        _damage = (_stat.GetStat(EStatType.MeleeDamage) + _stat.GetStat(EStatType.MagicDamage)) * _stat.GetStat(EStatType.TotalDamage);
        _controller.LastAttackTime = Machine.Runner.LocalRenderTime;
        _controller.PlayAnimTriggerNetwork(EAnimTrigger.Attack);
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
        _animationFinished = true;
    }
}
