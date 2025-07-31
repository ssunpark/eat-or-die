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
            () => _animationFinished
        );
    }
    protected override void OnEnterState()
    {
        _animationFinished = false;
        _controller.PlayAnimTriggerNetwork(EAnimTrigger.Cook);
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
