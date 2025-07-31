using Fusion.Addons.FSM;
public class PlayerHitState : APlayerStateBase, IAnimationActionEndNotify
{
    public PlayerHitState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Hit;
    }
    private bool _animationFinished;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished
        );
    }
    protected override bool CanExitState(IState nextState)
    {
        return _animationFinished;
    }
    protected override void OnEnterState()
    {
        _controller.PlayAnimTriggerNetwork(EAnimTrigger.Hit);
    }

    public void OnAnimationFinished()
    {
        _animationFinished = true;
    }
}
