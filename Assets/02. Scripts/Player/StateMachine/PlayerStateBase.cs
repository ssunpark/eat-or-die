public abstract class PlayerStateBase
{
    protected PlayerStateMachine _fsm;
    protected PlayerController _controller;
    protected StatManager _stat;
    protected ResourceManager _resource;
    public virtual bool CanMove => false;
    public PlayerStateBase(PlayerStateMachine fsm, PlayerController controller)
    {
        _fsm = fsm;
        _controller = controller;
        _stat = controller.Stat;
        _resource = controller.Resource;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}