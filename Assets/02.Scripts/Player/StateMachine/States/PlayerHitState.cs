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
            () => _animationFinished && _controller.FSM.ActiveState == _controller.FSMStateInstances.Hit
        );
    }
    protected override bool CanExitState(IState nextState)
    {
        return _animationFinished;
    }
    protected override void OnEnterState()
    {
        _animationFinished = false;
    }

    protected override void OnEnterStateRender()
    {
        _controller.PlayAnimTrigger(EAnimTrigger.Hit);
    }

    public void OnAnimationFinished()
    {
        if(_controller.HasStateAuthority)
            _animationFinished = true;
    }
}
