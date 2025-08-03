using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
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
            () => _animationFinished
        );
    }
    protected override void OnEnterState()
    {
        _animationFinished = false;
        _controller.PlayAnimTriggerNetwork(EAnimTrigger.Interact);
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
        _animationFinished = true;
    }
}