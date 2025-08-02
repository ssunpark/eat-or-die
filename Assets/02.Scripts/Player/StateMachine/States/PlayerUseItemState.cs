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
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished);
    }
    protected override void OnEnterStateRender()
    {
        _animationFinished = false;
        _controller.PlayAnimTrigger(EAnimTrigger.UseItem);
    }

    protected override bool CanExitState(IState nextState)
    {
        return _animationFinished;
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        if (_controller.HasInputAuthority)
            _controller.ItemHolder.UseItem(_controller.UseTarget);
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        Debug.Log("PlayerUseItemState: Animation finished");
        _animationFinished = true;
    }
}