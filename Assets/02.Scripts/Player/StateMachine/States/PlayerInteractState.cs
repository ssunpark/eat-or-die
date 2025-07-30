using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
public class PlayerInteractState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerInteractState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Interact;
    }

    protected override void OnEnterState()
    {
        if (_controller.Object.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Interact);

            _controller.RPC_SetMoveFlag(true);
        }
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        if (_controller.StateValues.Interactable != null)
        {
            _controller.Interact.UseOrInteract(
                interactable: _controller.StateValues.Interactable
            );
        }
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        Machine.ForceActivateState(_controller.FSMStateInstances.Idle);
        _controller.RPC_SetMoveFlag(false);
    }
}