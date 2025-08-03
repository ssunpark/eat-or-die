using Fusion.Addons.FSM;
public class PlayerRecoverState : APlayerStateBase, IAnimationActionEndNotify
{

    public PlayerRecoverState(PlayerController controller) : base(controller)
    {
    }
    private bool _animationFinished;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished
        );
    }
    public void OnAnimationFinished()
    {
        _animationFinished = true;
    }

    protected override void OnEnterState()
    {
        _controller.PlayAnimTriggerNetwork(EAnimTrigger.Recover);
        _resource.RestoreHunger(_resource.MaxHunger / 20);
    }
    protected override void OnExitState()
    {
    }

}
