using Fusion.Addons.FSM;
public class BerserkIdle : ABerserkSubStateBase, IAnimationActionEndNotify
{
    private bool _animFinished;

    public BerserkIdle(PlayerController controller) : base(controller) { }

    protected override void OnInitialize()
    {
        this.AddTransition(
            Machine.GetState<BerserkChase>(),
            () => _animFinished && _controller.HasStateAuthority
        );
    }

    protected override bool CanExitState(IState nextState)
    {
        return _animFinished;
    }

    protected override void OnEnterState()
    {
        _animFinished = false;
    }

    public void OnAnimationFinished()
    {
        _animFinished = true;
    }
}