public abstract class PlayerStateBase
{
    protected PlayerStateMachine _fsm;
    protected PlayerController _controller;

    public PlayerStateBase(PlayerStateMachine fsm, PlayerController controller)
    {
        _fsm = fsm;
        _controller = controller;
    }

    public virtual void TryJump(NetworkInputData input)
    {
        if (input.isJumping && _controller.IsGrounded)
            _controller.Jump();
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}