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

    public virtual void TryJump(NetworkInputData input)
    {
        if (input.isJumping && _controller.IsGrounded)
        {
            float jumpPower = _stat.GetStat(EStatType.JumpPower);
            _controller.Jump(jumpPower);
        }
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}