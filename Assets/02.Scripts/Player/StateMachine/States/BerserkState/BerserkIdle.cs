using Fusion.Addons.FSM;
public class BerserkIdle : ABerserkSubStateBase
{
    private bool _animFinished;

    public BerserkIdle(PlayerFSM controller) : base(controller) { }

    protected override bool CanExitState(IState nextState)
    {
        return _animFinished;
    }

    protected override void OnEnterState()
    {
        _animFinished = false;
    }

    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime("Berserk Start", AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths["Berserk Start"])
        {
            Machine.ForceActivateState<BerserkChase>();
        }
    }
}