using UnityEngine;

public abstract class APlayerState
{
    protected PlayerStateMachine _fsm;
    protected PlayerController _controller;
    protected StatManager _stat;
    protected ResourceManager _resource;
    public virtual bool CanMove => false;
    public virtual bool CanAct => false;

    protected bool CanAttack => _controller.LastAttackTime + 1 / Mathf.Max(_stat.GetStat(EStatType.AttackSpeed), 0.001f) < _fsm.Runner.LocalRenderTime;
    public APlayerState(PlayerStateMachine fsm, PlayerController controller)
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