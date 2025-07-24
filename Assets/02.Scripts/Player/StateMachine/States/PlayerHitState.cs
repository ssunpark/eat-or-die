using UnityEngine;

public class PlayerHitState : APlayerState
{
    private float _hitStunDuration;
    private float _elapsed;

    public PlayerHitState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }

    public override void Enter()
    {
        _elapsed = 0f;
        _hitStunDuration = 0.5f; // 경직 시간
        if (_controller.Object.HasInputAuthority)
        {
            //_controller.Rpc_PlayAnimTrigger(EAnimTrigger.Hit);
        }

    }

    public override void Tick()
    {
        _elapsed += _fsm.Runner.DeltaTime;

        if (_elapsed >= _hitStunDuration)
        {
            _fsm.ChangeState(EPlayerState.Idle);
        }
    }

    public override void Exit()
    {
    }
}
