using Fusion.Addons.FSM;
public class PlayerRecoverState : APlayerStateBase, IAnimationActionEndNotify
{

    public PlayerRecoverState(PlayerFSM controller) : base(controller)
    {
    }
    private bool _animationFinished;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished && _controller.HasStateAuthority
        );
    }


    public void OnAnimationFinished()
    {
        _animationFinished = true;
    }

    protected override void OnEnterState()
    {
        _resource.RestoreHunger(_resource.MaxHunger / 20);
    }

    protected override void OnEnterStateRender()
    {
        _animationFinished = false;
        _controller.PlayAnimTrigger(EAnimTrigger.Recover);
    }
    protected override void OnExitState()
    {
    }

}
