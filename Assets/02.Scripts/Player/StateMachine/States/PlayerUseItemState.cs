using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
public class PlayerUseItemState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerUseItemState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.UseItem;

    }
    private bool _animationFinished;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished
        );
    }
    protected override void OnEnterState()
    {
        _animationFinished = false;
        _controller.PlayAnimTriggerNetwork(EAnimTrigger.UseItem);
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
        _animationFinished = true;
    }
}