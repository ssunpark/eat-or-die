using UnityEngine;
using Fusion;
public class PlayerCookingState : APlayerStateBase, IAnimationActionEndNotify, IAnimationActionNotify
{
    public PlayerCookingState(PlayerController controller) : base(controller) 
    {
        StateId = (int)EPlayerState.Cooking;
    }
    protected override void OnEnterState()
    {
        if (_controller.Object.HasInputAuthority)
        {
            _controller.RPC_SetMoveFlag(true);
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Cook);
        }
    }

    protected override void OnExitState()
    {
        if (_controller.Object.HasInputAuthority)
        {
            CookingManager.Instance.OnCookingCompleted();
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.CookDone);

            _controller.RPC_SetMoveFlag(false);
        }
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        if (_controller.Object.HasInputAuthority)
        {
            CookingManager.Instance.OnCookingCompleted();
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.CookDone);
        }
    }
}
