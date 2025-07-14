public abstract class PlayerStateBase
{
    protected PlayerStateMachine fsm;
    protected PlayerController controller;

    public PlayerStateBase(PlayerStateMachine fsm, PlayerController controller)
    {
        this.fsm = fsm;
        this.controller = controller;
    }


    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}