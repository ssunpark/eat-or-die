using Fusion.Addons.FSM;
public class PlayerCookingState : APlayerStateBase, IAnimationActionEndNotify, IAnimationActionNotify
{
    public PlayerCookingState(PlayerController controller) : base(controller) 
    {
        StateId = (int)EPlayerState.Cooking;
    }
    private bool _animationFinished;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished && _controller.HasStateAuthority
        );
    }
    protected override void OnEnterState()
    {
        _animationFinished = false;
    }
    protected override void OnEnterStateRender()
    {
        _controller.PlayAnimTrigger(EAnimTrigger.Cook);
    }
    protected override bool CanExitState(IState nextState)
    {
        return _animationFinished;
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        if (_controller.Object.HasInputAuthority)
        {
            CookingManager.Instance.OnCookingCompleted();
        }

        _animationFinished = true;
    }
}
