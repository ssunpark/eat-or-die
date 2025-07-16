public abstract class PlayerStateBase
{
    protected PlayerStateMachine _fsm;
    protected PlayerController _controller;
    protected PlayerStat _stat;

    public PlayerStateBase(PlayerStateMachine fsm, PlayerController controller)
    {
        _fsm = fsm;
        _controller = controller;
        _stat = controller.PlayerStat;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}