using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
public class PlayerUseItemState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerUseItemState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.UseItem;

    }
    protected override void OnEnterState()
    {
        if (_controller.Object.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.UseItem);
            _controller.RPC_SetMoveFlag(true);
        }
    }

    protected override void OnExitState()
    {
        if (_controller.Object.HasInputAuthority)
            _controller.RPC_SetMoveFlag(false);
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        Debug.Log("PlayerUseItemState.OnActiodwnMoment");
        _controller.Interact.UseOrInteract(
                    usable: _controller.StateValues.Usable
                );
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        Machine.ForceActivateState(_controller.FSMStateInstances.Idle);
    }
}