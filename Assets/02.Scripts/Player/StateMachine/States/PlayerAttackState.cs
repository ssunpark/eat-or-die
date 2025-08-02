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
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Move,CanExitToMoveState);

        this.AddTransition(
            _controller.FSMStateInstances.Idle, CanExitToIdleState);
    }

    protected bool CanExitToMoveState()
    {
        if (!_controller.HasStateAuthority) return false;
        if (!hasFinishedAnimation) return false;
        if (!TryCacheInput()) return false;

        return _input.direction.sqrMagnitude > 0.01f;
    }

    protected bool CanExitToIdleState()
    {
        if (!_controller.HasStateAuthority) return false;
        if (!hasFinishedAnimation) return false;
        if (!TryCacheInput()) return false;
        return _input.direction.sqrMagnitude <= 0.01f;
    }
    protected override bool CanExitState(IState nextState)
    {
        return _animationFinished;
    }
    protected override void OnEnterState()
    {
        _animationFinished = false;
        _damage = (_stat.GetStat(EStatType.MeleeDamage) + _stat.GetStat(EStatType.MagicDamage)) * _stat.GetStat(EStatType.TotalDamage);
        _controller.LastAttackTime = Machine.Runner.LocalRenderTime;
        

        _controller.SetMoveFlagNetwork(true);
    }

    protected override void OnEnterStateRender()
    {
        _controller.PlayAnimTrigger(EAnimTrigger.Attack);
    }

    protected override void OnExitState()
    {
        _controller.SetMoveFlagNetwork(false);
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
