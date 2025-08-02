using Fusion.Addons.FSM;
using UnityEngine; // Add Fusion FSM for PlayerStateMachine
public class PlayerInteractState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerInteractState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Interact;
    }

    private bool _animationFinished;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished && _controller.HasStateAuthority
        );
    }

    protected override void OnEnterStateRender()
    {
        _animationFinished = false;
        _controller.PlayAnimTrigger(EAnimTrigger.Interact);
    }
    protected override bool CanExitState(IState nextState)
    {
        return _animationFinished;
    }

    protected override void OnFixedUpdate()
    {
    }
    void IAnimationActionNotify.OnActionMoment()
    {
        if(_controller.HasInputAuthority)
            _controller.Interact.Interact(_controller.InteractTarget);
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        _animationFinished = true;
    }
}