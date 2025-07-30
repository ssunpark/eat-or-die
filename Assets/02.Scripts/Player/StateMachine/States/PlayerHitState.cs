using UnityEngine;
using Fusion.Addons.FSM;
public class PlayerHitState : APlayerStateBase, IAnimationActionEndNotify
{
    public PlayerHitState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Hit;
    }

    protected override void OnEnterState()
    {
        //애니메이션이 아직 없어서 바로 Idle로 전환
        Machine.ForceActivateState<PlayerIdleState>();
        return;


        if (_controller.Object.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Hit);
            _controller.RPC_SetMoveFlag(true);
        }

    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {

        _controller.RPC_SetMoveFlag(false);
        Machine.ForceActivateState<PlayerIdleState>();
    }
}
