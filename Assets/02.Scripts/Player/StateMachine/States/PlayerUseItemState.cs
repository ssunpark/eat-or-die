using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
using UnityEngine;
public class PlayerUseItemState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerUseItemState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.UseItem;

    }
    private bool _animationFinished;
    private GameObject _target;
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
        if (_controller.CanUseHeldItem(out GameObject target))
        {
            _target = target;

            _controller.ItemHolder.UseItem(_target);
        }
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        _animationFinished = true;
    }
}